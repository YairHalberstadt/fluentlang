using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static FluentLang.Compiler.Emit.CSharpNameEscaper;
using Version = FluentLang.Compiler.Symbols.Interfaces.Version;

namespace FluentLang.Compiler.Emit
{
	public partial class FluentlangToCSharpEmitter
	{
		public void Emit(IAssembly assembly, TextWriter textWriter)
		{
			new Impl().Emit(assembly, textWriter);
		}

		/// <summary>
		/// Non thread safe implementation of FluentlangToCSharpEmitter
		/// </summary>
		private class Impl
		{
			private readonly MethodKeyGenerator _keyGenerator = new MethodKeyGenerator();
			private readonly UniqueIdentifierGenerator _uniqueIdentifierGenerator = new UniqueIdentifierGenerator();

			public void Emit(IAssembly assembly, TextWriter textWriter)
			{
				textWriter.WriteLine("using FluentLang.Runtime;");
				textWriter.WriteLine("using FluentLang.Runtime.Metadata;");
				textWriter.WriteLine("using System;");
				textWriter.WriteLine("using AssemblyFileVersionAttribute = System.Reflection.AssemblyFileVersionAttribute;");

				MetadataEmitter.EmitAssemblyNameAttribute(assembly.Name, textWriter);
				MetadataEmitter.EmitAssemblyFileVersionAttribute(assembly.Version, textWriter);

				foreach (var dependency in assembly.ReferencedAssembliesAndSelf.Where(x => x.Name != assembly.Name))
				{
					MetadataEmitter.EmitDependencyAttribute(dependency, textWriter);
				}

				foreach (var @interface in assembly.Interfaces.Where(x => x.IsExported))
				{
					MetadataEmitter.EmitInterfaceAttribute(@interface, textWriter);
				}

				textWriter.Write("public static class ");
				textWriter.Write(Utils.GetAssemblyLevelMethodsClassName(assembly.Name.ToString()));
				textWriter.Write("{");
				textWriter.WriteLine();
				foreach (var method in assembly.Methods)
				{
					Emit(method, textWriter);
				}
				textWriter.WriteLine("}");
				textWriter.Flush();
			}

			private void Emit(IMethod method, TextWriter textWriter)
			{
				if (method.IsExported)
				{
					MetadataEmitter.EmitMethodAttribute(method, textWriter);
				}
				if (method.DeclaringMethod is null)
				{
					if (method.IsExported)
						textWriter.Write("public ");
					textWriter.Write("static ");
				}
				Emit(method.ReturnType, textWriter);
				textWriter.Write(' ');
				if (method.DeclaringMethod is null)
				{
					textWriter.Write(Escape(method.FullyQualifiedName));
				}
				else
				{
					textWriter.Write(method.Name);
				}

				if (!method.TypeParameters.IsEmpty)
				{
					textWriter.Write("<");
					for (var i = 0; i < method.TypeParameters.Length; i++)
					{
						if (i != 0)
							textWriter.Write(",");
						var typeParameter = method.TypeParameters[i];
						textWriter.Write(typeParameter.Name);
					}
					textWriter.Write(">");
				}

				textWriter.Write('(');
				for (var i = 0; i < method.Parameters.Length; i++)
				{
					if (i != 0)
						textWriter.Write(",");
					var parameter = method.Parameters[i];
					Emit(parameter.Type, textWriter);
					textWriter.Write(' ');
					textWriter.Write(parameter.Name);

				}

				var paramNames = _keyGenerator.EnterMethodAndReturnMethodKeyParamNames(method);

				var isFirst = true;
				foreach (var paramName in paramNames)
				{
					if (!isFirst || !method.Parameters.IsEmpty)
					{
						textWriter.Write(", ");
					}
					isFirst = false;
					textWriter.Write("MethodKey ");
					textWriter.Write(paramName);
				}

				textWriter.WriteLine(")");

				foreach (var tp in method.TypeParameters)
				{
					if (tp.ConstrainedTo is { } constrainedTo)
					{
						textWriter.Write(" where ");
						Emit(tp, textWriter);
						textWriter.Write(" : ");
						Emit(constrainedTo, textWriter);
					}
				}

				textWriter.WriteLine("{");

				foreach (var statement in method.Statements)
				{
					Emit(statement, method.ReturnType, textWriter);
					textWriter.WriteLine();
				}

				foreach (var localMethod in method.LocalMethods)
				{
					Emit(localMethod, textWriter);
					textWriter.WriteLine();
				}

				textWriter.Write("}");

				_keyGenerator.ExitMethod(method);
			}

			private void Emit(IStatement statement, IType returnType, TextWriter textWriter)
			{
				if (statement is IDeclarationStatement declarationStatement)
				{
					if (declarationStatement.IdentifierName is { } identifierName)
					{
						Emit(declarationStatement.Type, textWriter);
						textWriter.Write(" ");
						textWriter.Write(identifierName);
					}
					else
					{
						textWriter.Write("_");
					}
					textWriter.Write(" = ");

					Emit(declarationStatement.Expression, declarationStatement.Type, textWriter);
				}
				else if (statement is IReturnStatement returnStatement)
				{
					textWriter.Write("return ");
					Emit(returnStatement.Expression, returnType, textWriter);
				}
				else throw Release.Fail($"unexpected type of statement: {statement.GetType()}");

				textWriter.Write(";");
			}

			private void Emit(IExpression expression, IType? targetType, TextWriter textWriter)
			{
				EmitUpcastIfNecessary(expression.Type, targetType ?? expression.Type, textWriter, () =>
				{
					textWriter.Write("(");
					switch (expression)
					{
						case INewObjectExpression noe: Emit(noe, textWriter); break;
						case IObjectPatchingExpression ope: Emit(ope, textWriter); break;
						case IBinaryOperatorExpression boe: Emit(boe, textWriter); break;
						case IPrefixUnaryOperatorExpression puoe: Emit(puoe, textWriter); break;
						case ILiteralExpression le: Emit(le, textWriter); break;
						case IStaticInvocationExpression sie: Emit(sie, textWriter); break;
						case IMemberInvocationExpression mie: Emit(mie, textWriter); break;
						case IConditionalExpression ce: Emit(ce, textWriter); break;
						case ILocalReferenceExpression lre: Emit(lre, textWriter); break;
						case IMatchExpression me: Emit(me, textWriter); break;
						default: throw Release.Fail($"unexpected type of expression: {expression.GetType()}");
					}
					textWriter.Write(")");
				});
			}

			private void Emit(IMatchExpression me, TextWriter textWriter)
			{
				Emit(me.Expression, null, textWriter);
				textWriter.WriteLine(" switch {");

				var matchingOptionsIdentifier = _uniqueIdentifierGenerator.GenerateIdentifier();
				foreach (var arm in me.Arms)
				{
					//Union { Inner: _, MatchingOptions: var i1}
					textWriter.Write("Union { Inner: ");
					if (arm.Local is { Identifier: var identifier, Type: var type })
					{
						Emit(type, textWriter);
						textWriter.Write(" ");
						textWriter.Write(identifier);
					}
					else
					{
						textWriter.Write("_");
					}
					textWriter.Write(", MatchingOptions: var ");
					textWriter.Write(matchingOptionsIdentifier);
					textWriter.Write("} when (");
					textWriter.Write(matchingOptionsIdentifier);
					textWriter.Write("& (ulong)");
					textWriter.Write(GetSubtypeOptions(arm.Type, (IUnion)me.Expression.Type));
					textWriter.Write(") != 0 => ");
					Emit(arm.Expression, me.Type, textWriter);
					textWriter.Write(",");
				}
				textWriter.Write("}");
			}

			private void EmitUpcastIfNecessary(IType type, IType targetType, TextWriter textWriter, Action emitInner)
			{
				if (type is IInterface && targetType is IInterface
					|| type is Primitive && targetType is Primitive
					|| type is ITypeParameter && targetType is ITypeParameter && type.IsEquivalentTo(targetType)
					|| type is IUnion && targetType is IUnion && type.IsEquivalentTo(targetType))
				{
					emitInner();
				}
				else if (type is IUnion && targetType is IInterface)
				{
					textWriter.Write("((FLObject)");
					emitInner();
					textWriter.Write(".Inner)");
				}
				else if (type is IUnion && targetType is Primitive primitive)
				{
					textWriter.Write("((");
					textWriter.Write(primitive.ToString());
					textWriter.Write(")");
					emitInner();
					textWriter.Write(".Inner)");
				}
				else if (type is IUnion && targetType is ITypeParameter typeParameter)
				{
					textWriter.Write("((");
					textWriter.Write(typeParameter.Name);
					textWriter.Write(")");
					emitInner();
					textWriter.Write(".Inner)");
				}
				else if ((type is IInterface || type is Primitive || type is ITypeParameter) && targetType is IUnion union)
				{
					textWriter.Write("(new Union(");
					emitInner();
					textWriter.Write(", (ulong)");
					textWriter.Write(GetSupertypeOptions(type, union));
					textWriter.Write("))");
				}
				else if (type is ITypeParameter && targetType is IInterface)
				{
					textWriter.Write("((FLObject)");
					emitInner();
					textWriter.Write(")");
				}
				else if (type is ITypeParameter && targetType is Primitive primitive1)
				{
					textWriter.Write("((");
					textWriter.Write(primitive1.ToString());
					textWriter.Write(")");
					emitInner();
					textWriter.Write(")");
				}
				else if (type is ITypeParameter && targetType is ITypeParameter typeParameter1)
				{
					textWriter.Write("((");
					textWriter.Write(typeParameter1.Name);
					textWriter.Write(")");
					emitInner();
					textWriter.Write(")");
				}
				else if (type is IUnion sourceUnion && targetType is IUnion targetUnion)
				{
					textWriter.Write("(new Union(");
					emitInner();
					textWriter.Write(", stackalloc ulong[] {");
					var any = false;
					foreach (var option in sourceUnion.Options)
					{
						if (any)
						{
							textWriter.Write(",");
						}
						any = true;
						textWriter.Write(GetSupertypeOptions(option, targetUnion));
					}
					textWriter.Write("}))");
				}
				else
				{
					Release.Fail("unreachable");
				}
			}

			private static ulong GetSupertypeOptions(IType type, IUnion union)
			{
				ulong bits = 0;
				for (var i = 0; i < union.Options.Length; i++)
				{
					var option = union.Options[i];
					if (type.IsSubtypeOf(option))
					{
						bits |= ((ulong)1) << i;
					}
				}

				return bits;
			}

			private static ulong GetSubtypeOptions(IType type, IUnion union)
			{
				ulong bits = 0;
				for (var i = 0; i < union.Options.Length; i++)
				{
					var option = union.Options[i];
					if (option.IsSubtypeOf(type))
					{
						bits |= ((ulong)1) << i;
					}
				}

				return bits;
			}

			private void Emit(ILocalReferenceExpression lre, TextWriter textWriter)
			{
				textWriter.Write(lre.Identifier);
			}

			private void Emit(IConditionalExpression ce, TextWriter textWriter)
			{
				Emit(ce.Condition, null, textWriter);
				textWriter.Write(" ? ");
				Emit(ce.IfTrue, ce.Type, textWriter);
				textWriter.Write(" : ");
				Emit(ce.IfFalse, ce.Type, textWriter);
			}

			private void Emit(IMemberInvocationExpression mie, TextWriter textWriter)
			{
				var method = mie.Method;
				var methodKey = _keyGenerator.GenerateMethodKey(method);
				Emit(mie.Expression, null, textWriter);
				textWriter.Write(".GetMethod<");
				EmitDelegateType(method.Parameters, method.ReturnType, textWriter);
				textWriter.Write(">(");
				EmitMethodKey(methodKey, textWriter);
				textWriter.Write(")(");
				EmitArguments(mie.Arguments, mie.Method.Parameters, textWriter);
				textWriter.Write(")");
			}

			private static void EmitMethodKey(MethodKeyOrParamName methodKey, TextWriter textWriter)
			{
				if (methodKey is MethodKeyOrParamName.MethodKey { Value: var val })
				{
					textWriter.Write("new MethodKey(\"");
					textWriter.Write(val);
					textWriter.Write("\")");
				}
				else if (methodKey is MethodKeyOrParamName.ParamName { Value: var paramName })
				{
					textWriter.Write(paramName);
				}
				else
				{
					throw Release.Fail("This location is thought to be unreachable");
				}
			}

			private void EmitArguments(IEnumerable<IExpression> arguments, IEnumerable<IParameter> parameters, TextWriter textWriter)
			{
				var isFirst = true;
				foreach (var (argument, parameter) in arguments.Zip(parameters, (a,b) => (a,b)))
				{
					if (isFirst)
					{
						isFirst = false;
					}
					else
					{
						textWriter.Write(",");
					}
					Emit(argument, parameter.Type, textWriter);
				}
			}

			private void EmitDelegateType(IEnumerable<IParameter> parameters, IType returnType, TextWriter textWriter)
			{
				textWriter.Write("Func<");
				foreach (var parameter in parameters)
				{
					Emit(parameter.Type, textWriter);
					textWriter.Write(",");
				}
				Emit(returnType, textWriter);
				textWriter.Write('>');
			}

			private void Emit(IStaticInvocationExpression sie, TextWriter textWriter)
			{
				var method = sie.Method;
				EmitMethodName(method, textWriter);
				EmitTypeArguments(sie.TypeArguments, textWriter);
				textWriter.Write("(");
				EmitArguments(sie.Arguments, sie.Method.Parameters, textWriter);
				EmitRequiredMethodKeyArguments(sie.Method, textWriter);
				textWriter.Write(")");
			}

			private void EmitTypeArguments(ImmutableArray<IType> typeArguments, TextWriter textWriter)
			{
				if (typeArguments.Length > 0)
				{
					textWriter.Write("<");
					for (var i = 0; i < typeArguments.Length; i++)
					{
						if (i != 0)
							textWriter.Write(", ");
						Emit(typeArguments[i], textWriter);
					}
					textWriter.Write(">");
				}
			}

			private void EmitRequiredMethodKeyArguments(IMethod method, TextWriter textWriter)
			{
				var isFirst = true;
				foreach (var requiredMethod in method.RequiredMethodKeys)
				{
					if (!isFirst || !method.Parameters.IsEmpty)
					{
						textWriter.WriteLine(", ");
					}
					isFirst = false;

					MethodKeyOrParamName methodKeyOrParamName;
					if (requiredMethod.TryGetMethod(out var m))
					{
						methodKeyOrParamName = _keyGenerator.GenerateMethodKeyExcludingFirstParameter(m);
					}
					else if (requiredMethod.TryGetInterfaceMethod(out var im))
					{
						methodKeyOrParamName = _keyGenerator.GenerateMethodKey(im);
					}
					else
					{
						throw Release.Fail("This location is thought to be unreachable");
					}
					EmitMethodKey(methodKeyOrParamName, textWriter);
				}
			}

			private static void EmitMethodName(IMethod method, TextWriter textWriter)
			{
				if (method.DeclaringMethod is null)
				{
					textWriter.Write(Utils.GetAssemblyLevelMethodsClassName(method.DeclaringAssembly.Name.ToString()));
					textWriter.Write(".");
					textWriter.Write(Escape(method.FullyQualifiedName));
				}
				else
				{
					textWriter.Write(method.Name);
				}
			}

			private void Emit(ILiteralExpression le, TextWriter textWriter)
			{
				switch (le.Value)
				{
					case int i: textWriter.Write(i); break;
					case bool b: textWriter.Write(b ? "true" : "false"); break;
					case double d:
						switch (d)
						{
							case double.PositiveInfinity:
								textWriter.Write("double.PositiveInfinity");
								break;
							case double.NegativeInfinity:
								textWriter.Write("double.NegativeInfinity");
								break;
							case double.NaN:
								textWriter.Write("double.NaN");
								break;
							default:
								textWriter.Write(d);
								textWriter.Write('d');
								break;
						}
						break;
					case char c:
						textWriter.Write('\'');
						EmitCharLiteral(c, textWriter);
						textWriter.Write('\'');
						break;
					case string s:
						EmitStringLiteral(s, textWriter);
						break;
					case null: throw Release.Fail("this location is thought to be unreachable");
					default: throw Release.Fail($"unexpected type of literal: {le.Value.GetType()}");
				}
			}

			private static void EmitCharLiteral(char @char, TextWriter textWriter)
			{
				textWriter.Write(@"\u");
				textWriter.Write(((int)@char).ToString("X4"));
			}

			private static void EmitStringLiteral(string str, TextWriter textWriter)
			{
				textWriter.Write('\"');
				foreach (var c in str)
					EmitCharLiteral(c, textWriter);
				textWriter.Write('\"');
			}

			private void Emit(IPrefixUnaryOperatorExpression puoe, TextWriter textWriter)
			{
				Emit(puoe.Operator, textWriter);
				Emit(puoe.Expression, null, textWriter);
			}

			private void Emit(IBinaryOperatorExpression boe, TextWriter textWriter)
			{
				Emit(boe.Left, null, textWriter);
				if ((boe.Left.Type == Primitive.String || boe.Left.Type == Primitive.Bool)
					&& IsInequalityOperator(boe.Operator))
				{
					textWriter.Write(".CompareTo(");
					Emit(boe.Right, null, textWriter);
					textWriter.Write(")");
					Emit(boe.Operator, textWriter);
					textWriter.Write("0");
				}
				else
				{
					Emit(boe.Operator, textWriter);
					Emit(boe.Right, null, textWriter);
				}

				static bool IsInequalityOperator(Operator @operator)
				{
					switch (@operator)
					{
						case Operator.GreaterThan:
						case Operator.LessThan:
						case Operator.GreaterThanOrEqualTo:
						case Operator.LessThanOrEqualTo:
							return true;
						default: return false;
					}
				}
			}

			private void Emit(IObjectPatchingExpression ope, TextWriter textWriter)
			{
				var mixinIndex = 0;
				textWriter.Write("(((Func<FLObject,");
				var mixins =
					ope
					.Patches
					.OfType<IMixinPatch>()
					.Select(x => x.Expression)
					.ToList();
				foreach (var mie in mixins)
				{
					Emit(mie!.Type, textWriter);
					textWriter.Write(",");
				}
				textWriter.Write("FLObject>)((Original");
				foreach (var i in Enumerable.Range(0, mixins.Count))
				{
					textWriter.Write(", Mixin");
					textWriter.Write(i);
				}
				textWriter.Write(") => { FLObject Target = null; Target = Original");
				foreach (var patch in ope.Patches)
				{
					EmitPatch(patch, textWriter);
				}
				textWriter.Write("; return Target; })))(");
				Emit(ope.Expression, null, textWriter);
				foreach (var mie in mixins)
				{
					textWriter.Write(",");
					Emit(mie!, null, textWriter);
				}
				textWriter.Write(")");

				void EmitPatch(IObjectPatch patch, TextWriter textWriter)
				{
					if (patch is IMethodPatch { Method: var method, TypeArguments: var typeArguments })
					{
						textWriter.Write(".With(");
						EmitMethodKey(_keyGenerator.GenerateMethodKeyExcludingFirstParameter(method), textWriter);
						textWriter.Write(", (");
						EmitDelegateType(method.Parameters.Skip(1), method.ReturnType, textWriter);
						textWriter.Write(")((");
						var paramsCount = method.Parameters.Length - 1;
						foreach (var i in Enumerable.Range(0, paramsCount))
						{
							if (i != 0)
								textWriter.Write(",");
							textWriter.Write("Temp");
							textWriter.Write(i);
						}
						textWriter.Write(") => ");
						EmitMethodName(method, textWriter);
						EmitTypeArguments(typeArguments, textWriter);

						textWriter.Write("(");

						EmitUpcastIfNecessary(
							ope.Expression.Type,
							method.Parameters[0].Type,
							textWriter,
							() => textWriter.Write("Target"));

						foreach (var i in Enumerable.Range(0, paramsCount))
						{
							textWriter.Write(",");
							textWriter.Write("Temp");
							textWriter.Write(i);
						}

						EmitRequiredMethodKeyArguments(method, textWriter);

						textWriter.Write(")))");
					}
					else if (patch is IMixinPatch { Expression: { Type: IInterface type } })
					{
						foreach (var interfaceMethod in type.Methods)
						{
							var key = _keyGenerator.GenerateMethodKey(interfaceMethod);
							textWriter.Write(".With(");
							EmitMethodKey(key, textWriter);
							textWriter.Write(", Mixin");
							textWriter.Write(mixinIndex);
							textWriter.Write(".GetMethod<Delegate>(");
							EmitMethodKey(key, textWriter);
							textWriter.Write("))");
						}
					}
				}
			}

			private void Emit(INewObjectExpression _, TextWriter textWriter)
			{
				textWriter.Write("FLObject.Empty");
			}

			private void Emit(IType type, TextWriter textWriter)
			{
				if (type is IInterface)
					textWriter.Write("FLObject");
				else if (type is Primitive p)
					textWriter.Write(p.ToString());
				else if (type is IUnion)
					textWriter.Write("Union");
				else if (type is ITypeParameter tp)
					textWriter.Write(tp.Name);
				else
					Release.Fail("this location is thought to be unreachable");
			}

			private void Emit(Operator @operator, TextWriter textWriter)
			{
				textWriter.Write(@operator switch
				{
					Operator.Divide => "/",
					Operator.Equal => "==",
					Operator.GreaterThan => ">",
					Operator.GreaterThanOrEqualTo => ">=",
					Operator.LessThan => "<",
					Operator.LessThanOrEqualTo => "<=",
					Operator.Minus => "-",
					Operator.Multiply => "*",
					Operator.NotEqual => "!=",
					Operator.Plus => "+",
					Operator.Remainder => "%",
					_ => throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(Operator)),
				});
			}

			private static class MetadataEmitter
			{
				public static void EmitMethodAttribute(IMethod method, TextWriter textWriter)
				{
					textWriter.Write("[MethodSignature(\"");
					textWriter.Write(method.FullyQualifiedName.ToString());
					EmitTypeParameterList(method.TypeParameters, textWriter);
					EmitParameterList(method.Parameters, textWriter);
					textWriter.Write(":");
					Emit(method.ReturnType, textWriter);
					textWriter.Write("\", new string[]{ ");
					for(var i = 0; i < method.RequiredMethodKeys.Length; i++)
					{
						if (i != 0)
							textWriter.Write(", ");
						textWriter.Write("\"");
						EmitRequiredMethodKeySignature(method.RequiredMethodKeys[i], textWriter);
						textWriter.Write("\"");
					}
					textWriter.Write("})]");
				}

				private static void EmitRequiredMethodKeySignature(MethodOrInterfaceMethod methodOrInterfaceMethod, TextWriter textWriter)
				{
					if (methodOrInterfaceMethod.TryGetInterfaceMethod(out var interfaceMethod))
					{
						Emit(interfaceMethod, textWriter);
					}
					else if (methodOrInterfaceMethod.TryGetMethod(out var method))
					{
						textWriter.Write(method.Name);
						EmitParameterList(method.Parameters.Skip(1), textWriter);
						textWriter.Write(":");
						Emit(method.ReturnType, textWriter);
					}
				}

				private static void EmitParameterList(IEnumerable<IParameter> parameters, TextWriter textWriter)
                {
					textWriter.Write("(");
					var isFirst = true;
					foreach (var parameter in parameters)
					{
						if (!isFirst)
							textWriter.Write(", ");
						isFirst = false;
						Emit(parameter, textWriter);
					}
					textWriter.Write(")");
				}

				private static void EmitTypeParameterList(ImmutableArray<ITypeParameter> typeParameters, TextWriter textWriter)
				{
					if (typeParameters.Length > 0)
					{
						textWriter.Write("<");
						for (var i = 0; i < typeParameters.Length; i++)
						{
							if (i != 0)
								textWriter.Write(", ");
							EmitTypeParameter(typeParameters[i], textWriter);
						}
						textWriter.Write(">");
					}
				}

				public static void EmitInterfaceAttribute(IInterface @interface, TextWriter textWriter)
				{
					textWriter.Write("[assembly: Interface(\"");
					Release.Assert(@interface.FullyQualifiedName is { });
					textWriter.Write(@interface.FullyQualifiedName.ToString());
					textWriter.Write("\",\"");
					Emit(@interface, textWriter, isDeclaration: true);
					textWriter.Write("\",");
					Emit(@interface.TypeParameters, textWriter);
					textWriter.Write(")]");
				}

				private static void Emit(IParameter parameter, TextWriter textWriter)
				{
					textWriter.Write(parameter.Name);
					textWriter.Write(":");
					Emit(parameter.Type, textWriter);
				}

				private static void Emit(ImmutableArray<ITypeParameter> typeParameters, TextWriter textWriter)
				{
					textWriter.Write("new string[]{");
					for (var i = 0; i < typeParameters.Length; i++)
					{
						if (i != 0)
							textWriter.Write(",");
						textWriter.Write("\"");
						EmitTypeParameter(typeParameters[i], textWriter);
						textWriter.Write("\"");
					}
					textWriter.Write("}");
				}

				private static void EmitTypeParameter(ITypeParameter typeParameter, TextWriter textWriter)
				{
					textWriter.Write(typeParameter.Name);
					if (typeParameter.ConstrainedTo is { } constrainedTo)
					{
						textWriter.Write(":");
						Emit(constrainedTo, textWriter);
					}
				}

				private static void Emit(IType type, TextWriter textWriter)
				{
					if (type is Primitive primitive)
					{
						Emit(primitive, textWriter);
					}
					else if (type is IInterface @interface)
					{
						Emit(@interface, textWriter, isDeclaration: false);
					}
					else if (type is IUnion union)
					{
						Emit(union, textWriter);
					}
					else if (type is ITypeParameter typeParameter)
					{
						Emit(typeParameter, textWriter);
					}
					else
					{
						Release.Fail("This location is thought to be unreachable");
					}
				}

				private static void Emit(IInterface @interface, TextWriter textWriter, bool isDeclaration)
				{
					if (!isDeclaration && @interface.FullyQualifiedName is { } name && @interface.IsExported)
					{
						textWriter.Write(name.ToString());
						EmitTypeArguments(@interface.TypeArguments, textWriter);
					}
					else
					{
						textWriter.Write("{");
						foreach (var method in @interface.Methods)
						{
							Emit(method, textWriter);
							textWriter.Write("; ");
						}
						textWriter.Write("}");
					}
				}

				private static void EmitTypeArguments(ImmutableArray<IType> typeArguments, TextWriter textWriter)
				{
					if (typeArguments.Length > 0)
					{
						textWriter.Write("<");
						for (var i = 0; i < typeArguments.Length; i++)
						{
							if (i != 0)
								textWriter.Write(", ");
							Emit(typeArguments[i], textWriter);
						}
						textWriter.Write(">");
					}
				}

				private static void Emit(IInterfaceMethod method, TextWriter textWriter)
				{
					textWriter.Write(method.Name);
					EmitParameterList(method.Parameters, textWriter);
					textWriter.Write(":");
					Emit(method.ReturnType, textWriter);
				}

				private static void Emit(Primitive primitive, TextWriter textWriter)
				{
					textWriter.Write(primitive.ToString());
				}

				private static void Emit(IUnion union, TextWriter textWriter)
				{
					var any = false;
					foreach (var option in union.Options)
					{
						if (any)
						{
							textWriter.Write("|");
						}
						any = true;
						Emit(option, textWriter);
					}
				}

				private static void Emit(ITypeParameter typeParameter, TextWriter textWriter)
				{
					textWriter.Write(typeParameter.Name);
				}

				internal static void EmitAssemblyFileVersionAttribute(Version version, TextWriter textWriter)
				{
					textWriter.Write("[assembly: AssemblyFileVersion(");
					EmitStringLiteral(version.ToString(), textWriter);
					textWriter.WriteLine(")]");
				}

				internal static void EmitDependencyAttribute(IAssembly dependency, TextWriter textWriter)
				{
					textWriter.Write("[assembly: Dependency(");
					EmitStringLiteral(dependency.Name.ToString(), textWriter);
					textWriter.Write(",");
					EmitStringLiteral(dependency.Version.ToString(), textWriter);
					textWriter.WriteLine(")]");
				}

				internal static void EmitAssemblyNameAttribute(QualifiedName name, TextWriter textWriter)
				{
					textWriter.Write("[assembly: AssemblyName(");
					EmitStringLiteral(name.ToString(), textWriter);
					textWriter.WriteLine(")]");
				}
			}

			private class UniqueIdentifierGenerator
			{
				private int _index = 0;
				public string GenerateIdentifier() => "__i" + _index++;
			}
		}
	}
}
