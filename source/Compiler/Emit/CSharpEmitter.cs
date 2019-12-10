using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static FluentLang.Compiler.Emit.CSharpNameEscaper;

namespace FluentLang.Compiler.Emit
{
	internal class CSharpEmitter
	{
		private const string ASSEMBLY_LEVEL_METHODS = "_AssemblyLevelMethods";

		private readonly MethodKeyGenerator _keyGenerator;

		public CSharpEmitter(MethodKeyGenerator keyGenerator)
		{
			_keyGenerator = keyGenerator;
		}

		public void Emit(IAssembly assembly, TextWriter textWriter)
		{
			textWriter.WriteLine("using FluentLang.Runtime;");
			textWriter.WriteLine("using System;");

			textWriter.Write("public static class ");
			textWriter.Write(assembly.Name);
			textWriter.Write(ASSEMBLY_LEVEL_METHODS + "{");
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
			textWriter.WriteLine(") {");

			foreach (var statement in method.Statements)
			{
				Emit(statement, textWriter);
				textWriter.WriteLine();
			}

			foreach (var localMethod in method.LocalMethods)
			{
				Emit(localMethod, textWriter);
				textWriter.WriteLine();
			}

			textWriter.Write("}");
		}

		private void Emit(IStatement statement, TextWriter textWriter)
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

				Emit(declarationStatement.Expression, textWriter);
			}
			else if (statement is IReturnStatement returnStatement)
			{
				textWriter.Write("return ");
				Emit(returnStatement.Expression, textWriter);
			}
			else throw Release.Fail($"unexpected type of statement: {statement.GetType()}");

			textWriter.Write(";");
		}

		private void Emit(IExpression expression, TextWriter textWriter)
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
				default: throw Release.Fail($"unexpected type of expression: {expression.GetType()}");
			};
			textWriter.Write(")");
		}

		private void Emit(ILocalReferenceExpression lre, TextWriter textWriter)
		{
			textWriter.Write(lre.Identifier);
		}

		private void Emit(IConditionalExpression ce, TextWriter textWriter)
		{
			Emit(ce.Condition, textWriter);
			textWriter.Write(" ? ");
			Emit(ce.IfTrue, textWriter);
			textWriter.Write(" : ");
			Emit(ce.IfTrue, textWriter);
		}

		private void Emit(IMemberInvocationExpression mie, TextWriter textWriter)
		{
			var method = mie.Method;
			var methodKey = _keyGenerator.GenerateMethodKey(method);
			Emit(mie.Expression, textWriter);
			textWriter.Write(".GetMethod<");
			EmitDelegateType(method.Parameters, method.ReturnType, textWriter);
			textWriter.Write(">(");
			EmitMethodKey(methodKey, textWriter);
			textWriter.Write(")(");
			EmitArguments(mie.Arguments, textWriter);
			textWriter.Write(")");
		}

		private static void EmitMethodKey(string methodKey, TextWriter textWriter)
		{
			textWriter.Write("new MethodKey(\"");
			textWriter.Write(methodKey);
			textWriter.Write("\")");
		}

		private void EmitArguments(IEnumerable<IExpression> arguments, TextWriter textWriter)
		{
			var isFirst = true;
			foreach (var argument in arguments)
			{
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					textWriter.Write(",");
				}
				Emit(argument, textWriter);
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
			textWriter.Write("(");
			EmitArguments(sie.Arguments, textWriter);
			textWriter.Write(")");
		}

		private static void EmitMethodName(IMethod method, TextWriter textWriter)
		{
			if (method.DeclaringMethod is null)
			{
				textWriter.Write(method.DeclaringAssembly.Name);
				textWriter.Write(ASSEMBLY_LEVEL_METHODS + ".");
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
					EmitCharLiteral(c);
					textWriter.Write('\'');
					break;
				case string s:
					textWriter.Write('\"');
					foreach(var c in s)
						EmitCharLiteral(c);
					textWriter.Write('\"');
					break;
				case null: throw Release.Fail("this location is thought to be unreachable");
				default: throw Release.Fail($"unexpected type of literal: {le.Value.GetType()}");
			}

			void EmitCharLiteral(char c)
			{
				textWriter.Write(@"\u");
				textWriter.Write(((int)c).ToString("X4"));
			}
		}

		private void Emit(IPrefixUnaryOperatorExpression puoe, TextWriter textWriter)
		{
			Emit(puoe.Operator, textWriter);
			Emit(puoe.Expression, textWriter);
		}

		private void Emit(IBinaryOperatorExpression boe, TextWriter textWriter)
		{
			Emit(boe.Left, textWriter);
			if ((boe.Left.Type == Primitive.String || boe.Left.Type == Primitive.Bool) 
				&& IsInequalityOperator(boe.Operator))
			{
				textWriter.Write(".CompareTo(");
				Emit(boe.Right, textWriter);
				textWriter.Write(")");
				Emit(boe.Operator, textWriter);
				textWriter.Write("0");
			}
			else
			{
				Emit(boe.Operator, textWriter);
				Emit(boe.Right, textWriter);
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
				.Select(x => x.MixedInExpression)
				.Where(x => x != null)
				!.ToList<IExpression>();
			foreach(var mie in mixins)
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
			foreach(var patch in ope.Patches)
			{
				EmitPatch(patch, textWriter);
			}
			textWriter.Write("; return Target; })))(");
			Emit(ope.Expression, textWriter);
			foreach (var mie in ope.Patches.Select(x => x.MixedInExpression).Where(x => x != null))
			{
				textWriter.Write(",");
				Emit(mie!, textWriter);
			}
			textWriter.Write(")");

			void EmitPatch(IObjectPatch patch, TextWriter textWriter)
			{
				if (patch.Method is { } method)
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
					textWriter.Write("(Target");
					foreach (var i in Enumerable.Range(0, paramsCount))
					{
						textWriter.Write(",");
						textWriter.Write("Temp");
						textWriter.Write(i);
					}
					textWriter.Write(")))");
				}
				else if (patch.MixedInExpression is { } mixedInExpression)
				{

					foreach (var interfaceMethod in ((IInterface)mixedInExpression.Type).Methods)
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

		private void Emit(IType returnType, TextWriter textWriter)
		{
			if (returnType is IInterface)
				textWriter.Write("FLObject");
			else if (returnType is Primitive p)
				textWriter.Write(p.ToString());
			else throw Release.Fail("this location is thought to be unreachable");
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
	}
}
