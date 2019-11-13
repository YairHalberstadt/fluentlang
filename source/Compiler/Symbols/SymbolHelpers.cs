using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FluentLang.Compiler.Symbols
{
	public static class SymbolHelpers
	{
		public static bool TryFindBestType(IType a, IType b, [NotNullWhen(true)] out IType? type)
		{
			if (a.IsSubtypeOf(b))
			{
				type = b;
				return true;
			}
			if (b.IsSubtypeOf(a))
			{
				type = a;
				return true;
			}
			type = null;
			return false;
		}
	}
}
