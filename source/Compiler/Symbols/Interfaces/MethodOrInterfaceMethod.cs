using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

[DebuggerDisplay("{_method is null ? _interfaceMethod.Name : _method.FullyQualifiedName}")]
internal struct MethodOrInterfaceMethod : IEquatable<MethodOrInterfaceMethod>
{
	private readonly IMethod? _method;
	private readonly ImmutableArray<ITypeParameter> _typeArguments;
	private readonly IInterfaceMethod? _interfaceMethod;

	public MethodOrInterfaceMethod(IMethod method, ImmutableArray<IType> typeArguments) : this()
	{
		_method = method;
		_typeArguments = typeArguments.All(x => x is ITypeParameter)
			? typeArguments.Cast<ITypeParameter>().ToImmutableArray()
			: ImmutableArray<ITypeParameter>.Empty;
	}

	private MethodOrInterfaceMethod(IMethod method, ImmutableArray<ITypeParameter> typeArguments) : this()
	{
		_method = method;
		_typeArguments = typeArguments;
	}

	public MethodOrInterfaceMethod(IInterfaceMethod interfaceMethod) : this()
	{
		_interfaceMethod = interfaceMethod;
		_typeArguments = ImmutableArray<ITypeParameter>.Empty;
	}

	public bool TryGetMethod([MaybeNullWhen(false)] out IMethod method)
	{
		method = _method;
		return method != null;
	}

	public bool TryGetInterfaceMethod([MaybeNullWhen(false)] out IInterfaceMethod interfaceMethod)
	{
		interfaceMethod = _interfaceMethod;
		return interfaceMethod != null;
	}

	public MethodOrInterfaceMethod SubstituteTypeParameters(ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
	{
		if (_method != null)
		{
			var typeArguments = _typeArguments.Select(x => substitutions.TryGetValue(x, out var tp) ? tp as ITypeParameter : x).ToImmutableArray();
			if (typeArguments.Any(x => x is null))
				typeArguments = ImmutableArray<ITypeParameter>.Empty!;
			return new MethodOrInterfaceMethod(_method.Substitute(substitutions, substituted), typeArguments!);
		}
		if (_interfaceMethod != null)
		{
			return new MethodOrInterfaceMethod(_interfaceMethod.Substitute(substitutions, substituted));
		}
		throw Release.Fail("Do not use default constructor for " + nameof(MethodOrInterfaceMethod));
	}

	public override bool Equals(object? obj)
	{
		return obj is MethodOrInterfaceMethod method
			&& ReferenceEquals(_method, method._method)
			&& ReferenceEquals(_interfaceMethod, method._interfaceMethod);
	}

	public bool Equals(MethodOrInterfaceMethod other)
	{
		return ReferenceEquals(_method, other._method)
			&& ReferenceEquals(_interfaceMethod, other._interfaceMethod);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_method, _interfaceMethod);
	}

	public bool IsEquivalentToOriginal()
	{
		Release.Assert(_method != null);

		var originalTypeParameters = _method.OriginalDefinition.TypeParameters;
		if (_typeArguments.Length != originalTypeParameters.Length)
			return false;

		return _typeArguments.Zip(originalTypeParameters, (a, b) => a == b).All(x => x);
	}
}