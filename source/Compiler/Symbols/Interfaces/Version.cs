using System;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public class Version : IComparable<Version>
	{
		private readonly string _versionString;

		public Version(string versionString)
		{
			_versionString = versionString;
		}

		public int CompareTo(Version other)
		{
			return _versionString.CompareTo(other._versionString);
		}
	}
}

