using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public class Version : IEquatable<Version>
	{
		public Version(int major, int minor, string suffix = ""): this (major, minor, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), suffix)
		{ }


		internal Version(int major, int minor, long build, string suffix = "")
		{
			Major = major;
			Minor = minor;
			Build = build;
			Suffix = suffix;
		}

		public int Major { get; }
		public int Minor { get; }
		public long Build { get; }
		public string Suffix { get; }

		public DateTime Timestamp => DateTimeOffset.FromUnixTimeSeconds(Build).UtcDateTime;

		public override string ToString()
		{
			return Suffix is "" ? $"{Major}.{Minor}.{Build}" : $"{Major}.{Minor}.0-{Suffix}.{Build}";
		}

		public override bool Equals(object? obj)
		{
			return obj is Version version && Equals(version);
		}

		public bool Equals(Version? other)
		{
			if (other is null)
				return false;
			return
				ReferenceEquals(this, other)
				|| Major == other.Major
				&& Minor == other.Minor
				&& Build == other.Build
				&& Suffix == other.Suffix;

		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Major, Minor, Build, Suffix);
		}

		private static readonly Regex _versionRegex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)$");
		private static readonly Regex _preReleaseVersionRegex = new Regex("^(\\d+)\\.(\\d+)\\.0-(.*)\\.(\\d+)$");
		public static bool TryParse(string str, [NotNullWhen(true)] out Version? version)
		{
			var match = _versionRegex.Match(str);
			if (match.Success)
			{
				version = new Version(
					int.Parse(match.Groups[1].Value),
					int.Parse(match.Groups[2].Value),
					long.Parse(match.Groups[3].Value));
				return true;
			}
			match = _preReleaseVersionRegex.Match(str);
			if (match.Success)
			{
				version = new Version(
					int.Parse(match.Groups[1].Value),
					int.Parse(match.Groups[2].Value),
					long.Parse(match.Groups[4].Value),
					match.Groups[3].Value);
				return true;
			}
			version = null;
			return false;
		}
	}
}

