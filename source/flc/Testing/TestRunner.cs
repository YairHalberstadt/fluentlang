using FluentLang.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentLang.flc.Testing
{
	public class TestRunner
	{
		public static TestResult RunTests(Assembly compiledAssembly)
		{
			var methods =
				compiledAssembly
				.ExportedTypes
				.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
				.Where(x =>
					x.GetParameters().Length == 0
					&& (x.ReturnType == typeof(string) || x.ReturnType == typeof(bool)));

			var successes = new List<string>();
			var failures = new List<(string name, string message)>();
			foreach (var method in methods)
			{
				object? result;
				try
				{
					result = method.Invoke(null, null);
				}
				catch (Exception e)
				{
					result = e.ToString();
				}

				switch (result)
				{
					case true:
					case "":
					case null:
						successes.Add(method.Name);
						break;
					case false:
						failures.Add((method.Name, "failed"));
						break;
					case string message:
						failures.Add((method.Name, message));
						break;
					default:
						Release.Fail("This location is thought to be unreachable");
						break;
				}
			}

			return failures.Any()
				? (TestResult)new TestResult.Failure(successes, failures)
				: new TestResult.Success(successes);
		}
	}
}
