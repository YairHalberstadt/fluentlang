using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class ObjectPatchingExpression : SymbolBase, IObjectPatchingExpression
	{
		private readonly Object_patching_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IExpression> _expression;
		private readonly Lazy<ImmutableArray<IObjectPatch>> _patches;

		public ObjectPatchingExpression(
			Object_patching_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;
			_expression = new Lazy<IExpression>(BindExpression);
			_patches = new Lazy<ImmutableArray<IObjectPatch>>(BindPatches);
			Type = new PatchedType(this);
		}

		private IType BindType()
		{
			return new PatchedType(this);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private ImmutableArray<IObjectPatch> BindPatches()
		{
			return 
				_context
				.object_patch()
				.Select(x => new ObjectPatch(x, _methodBodySymbolContext, _diagnostics))
				.ToImmutableArray<IObjectPatch>();
		}

		public IExpression Expression => _expression.Value;

		public ImmutableArray<IObjectPatch> Patches => _patches.Value;

		public IType Type { get; }

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _expression.Value;
			_ = _patches.Value;
			Type.EnsureAllLocalDiagnosticsCollected();
		}

		private sealed class PatchedType : IInterface
		{
			private readonly ObjectPatchingExpression _objectPatchingExpression;
			private readonly Lazy<ImmutableArray<IInterfaceMethod>> _methods;

			public PatchedType(ObjectPatchingExpression objectPatchingExpression)
			{
				_objectPatchingExpression = objectPatchingExpression;
				_methods = new Lazy<ImmutableArray<IInterfaceMethod>>(GetMethods);
			}

			private ImmutableArray<IInterfaceMethod> GetMethods()
			{
				ImmutableArray<IInterfaceMethod> methods;
				var patchedInMethods = new List<(IMethod method, int index)>();

				var patchedMethods = _objectPatchingExpression.Patches.SelectMany((x, i) =>
				{
					if (x.Method is { } method)
					{
						if (method.Parameters.Length == 0)
						{
							_objectPatchingExpression._diagnostics.Add(new Diagnostic(
								new Location(_objectPatchingExpression._context.object_patch(i)),
								ErrorCode.CannotPatchInMethodWithoutParameters,
								ImmutableArray.Create<object?>(method)));
						}
						else
						{
							patchedInMethods.Add((method, i));
							return new[] { new PatchedInterfaceMethod(method) };
						}
					}
					else if (x.MixedInExpression is { } expression)
					{
						if (expression.Type is IInterface @interface)
						{
							return @interface.Methods;
						}
					}
					return Enumerable.Empty<IInterfaceMethod>();
				});

				if (_objectPatchingExpression.Expression.Type is IInterface @interface)
				{
					methods = @interface.Methods.Concat(patchedMethods).ToImmutableArray();
				}
				else
				{
					_objectPatchingExpression._diagnostics.Add(new Diagnostic(
						new Location(_objectPatchingExpression._context.expression()),
						ErrorCode.CanOnlyPatchInterface,
						ImmutableArray.Create<object?>(_objectPatchingExpression.Expression.Type)));

					methods = patchedMethods.ToImmutableArray();
				}

				// can't use `this`, as that would lead to infinite recursion
				var tempInterface = (IInterface)new TempInterface(methods);

				foreach (var (patchedInMethod, index) in patchedInMethods)
				{
					if (!tempInterface.IsSubtypeOf(patchedInMethod.Parameters[0].Type))
					{
						_objectPatchingExpression._diagnostics.Add(new Diagnostic(
							new Location(_objectPatchingExpression._context.object_patch(index)),
							ErrorCode.ResultantTypeOfObjectPatchingExpressionIsNotSubtypeOfFirstParameterOfPatchedInMethod,
							ImmutableArray.Create<object?>(patchedInMethod, this)));
					}
				}

				return methods;
			}

			public QualifiedName? FullyQualifiedName => null;

			public bool IsExported => false;

			public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

			public ImmutableArray<IInterfaceMethod> Methods => _methods.Value;

			public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

			void ISymbol.EnsureAllLocalDiagnosticsCollected() 
			{
				_ = _methods.Value;
			}

			private sealed class PatchedInterfaceMethod : IInterfaceMethod
			{
				private readonly IMethod _method;

				public PatchedInterfaceMethod(IMethod method)
				{
					_method = method;
					Parameters = method.Parameters.Skip(1).ToImmutableArray();
				}

				public string Name => _method.Name;

				public IType ReturnType => _method.ReturnType;

				public ImmutableArray<IParameter> Parameters { get; } //todo: make this lazy? probably not worth it.

				public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

				void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
			}

			private sealed class TempInterface : IInterface
			{
				public TempInterface(ImmutableArray<IInterfaceMethod> methods)
				{
					Methods = methods;
				}

				public QualifiedName? FullyQualifiedName => null;

				public bool IsExported => false;

				public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

				public ImmutableArray<IInterfaceMethod> Methods { get; }

				public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

				void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
			}
		}
	}
}
