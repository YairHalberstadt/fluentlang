using Xunit;
using Version = FluentLang.Compiler.Symbols.Interfaces.Version;

namespace FluentLang.Compiler.Tests.Unit.Symbols.Interfaces
{
	public class VersionTests
	{
		[Fact]
		public void CanParseVersion()
		{
			Assert.True(Version.TryParse("1.2.3", out var version));
			Assert.Equal(new Version(1, 2, 3), version);
		}

		[Fact]
		public void CanParseVersionWithLongBuild()
		{
			Assert.True(Version.TryParse($"1.2.{long.MaxValue}", out var version));
			Assert.Equal(new Version(1, 2, long.MaxValue), version);
		}

		[Fact]
		public void CanParsePrerealeaseVersion()
		{
			Assert.True(Version.TryParse("1.2.0-alpha.3", out var version));
			Assert.Equal(new Version(1, 2, 3, "alpha"), version);
		}

		[Fact]
		public void CanParsePrerealeaseVersionCOntaningExtraDashesDotsAndNumbers()
		{
			Assert.True(Version.TryParse("1.2.0-alpha-1.17.3", out var version));
			Assert.Equal(new Version(1, 2, 3, "alpha-1.17"), version);
		}

		[Theory]
		[InlineData("1")]
		[InlineData("1,2,3")]
		[InlineData("1a.2.3")]
		[InlineData("1..3")]
		[InlineData("1-alpha.1")]
		[InlineData("1,2,3-alpha.1")]
		[InlineData("1a.2.3-alpha.1")]
		[InlineData("1..3-alpha.1")]
		[InlineData("1.2.1-alpha.1")]
		[InlineData("1.2.0-alpha")]
		[InlineData("1.2.0-alpha1")]
		public void DoesNotParseInvalidVersions(string str)
		{
			Assert.False(Version.TryParse(str, out _));
		}
	}
}
