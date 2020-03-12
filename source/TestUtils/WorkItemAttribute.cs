using System;

namespace FluentLang.TestUtils
{
	/// <summary>
	/// Used to tag test methods or types which are created for a given WorkItem
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class WorkItemAttribute : Attribute
	{
		/// <param name="issueUri">The URI where the work item can be viewed.</param>
		public WorkItemAttribute(string issueUri)
		{
			IssueUri = issueUri;
		}

		public string IssueUri { get; }
	}
}