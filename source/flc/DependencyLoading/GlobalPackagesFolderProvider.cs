using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentLang.flc.DependencyLoading
{
	public static class GlobalPackagesFolderProvider
	{
		public static string GlobalPackagesFolder =>
			Environment.GetEnvironmentVariable("NUGET_PACKAGES", EnvironmentVariableTarget.Process)
			?? Environment.GetEnvironmentVariable("NUGET_PACKAGES", EnvironmentVariableTarget.User)
			?? Environment.GetEnvironmentVariable("NUGET_PACKAGES", EnvironmentVariableTarget.Machine)
			?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget/packages");
	}
}
