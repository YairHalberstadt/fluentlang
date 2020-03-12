using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

internal struct MethodOrInterfaceMethod : IEquatable<MethodOrInterfaceMethod>
{
	private readonly IMethod? _method;
	private readonly IInterfaceMethod? _interfaceMethod;

	public MethodOrInterfaceMethod(IMethod method) : this()
	{
		_method = method;
	}

	public MethodOrInterfaceMethod(IInterfaceMethod interfaceMethod) : this()
	{
		_interfaceMethod = interfaceMethod;
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
			return new MethodOrInterfaceMethod(_method.Substitute(substitutions, substituted));
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
}