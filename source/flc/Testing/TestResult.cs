using FluentLang.Shared;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.flc.Testing
{
	public abstract class TestResult
	{
		private TestResult() { }
		public class Success : TestResult
		{
			public Success(IEnumerable<string> succeeded)
			{
				Succeeded = succeeded;
			}

			public IEnumerable<string> Succeeded { get; }

			public override string GetMessage()
			{
				return $@"Test Run Succeeded:

{Succeeded.Count()} Successes:
{string.Join('\n', Succeeded)}";
			}

			public override TestResult Merge(TestResult other)
			{
				if (other is Success success)
					return new Success(Succeeded.Concat(success.Succeeded));
				else if (other is Failure failure)
				{
					return new Failure(Succeeded.Concat(failure.Succeeded), failure.Failed);
				}
				throw Release.Fail("This location is thought to be unreachable");
			}
		}

		public class Failure: TestResult
		{
			public Failure(IEnumerable<string> succeeded, IEnumerable<(string testName, string message)> failed)
			{
				Succeeded = succeeded;
				Failed = failed;
			}

			public IEnumerable<string> Succeeded { get; }
			public IEnumerable<(string testName, string message)> Failed { get; }

			public override TestResult Merge(TestResult other)
			{
				if (other is Success success)
					return new Failure(Succeeded.Concat(success.Succeeded), Failed);
				else if (other is Failure failure)
				{
					return new Failure(Succeeded.Concat(failure.Succeeded), Failed.Concat(failure.Failed));
				}
				throw Release.Fail("This location is thought to be unreachable");
			}

			public override string GetMessage()
			{
				return $@"Test Run Failed:

{Failed.Count()} Failures:
{string.Join('\n', Failed.Select(x => $"{x.testName}: {x.message}"))}

{Succeeded.Count()} Successes:
{string.Join('\n', Succeeded)}";
			}
		}

		public abstract TestResult Merge(TestResult other);

		public abstract string GetMessage();
	}
}
