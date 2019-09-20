using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentLang.Compiler.Diagnostics
{
	public class DiagnosticBag : IEnumerable<Diagnostic>
	{
		private DiagnosticBag? _parent;
		private readonly ConcurrentBag<Diagnostic> _topLevelDiagnostics = new ConcurrentBag<Diagnostic>();
		private readonly ConcurrentBag<DiagnosticBag> _childDiagnosticBags = new ConcurrentBag<DiagnosticBag>();

		public DiagnosticBag()
		{
		}

		private DiagnosticBag(DiagnosticBag parent)
		{
			_parent = parent;
		}

		private void AttachToParent()
		{
			if (_parent is null)
				return;

			_parent._childDiagnosticBags.Add(this);
			_parent.AttachToParent();
			_parent = null;
		}

		public void Add(Diagnostic diagnostic)
		{
			AttachToParent();
			_topLevelDiagnostics.Add(diagnostic);
		}

		public void AddRange(IEnumerable<Diagnostic> diagnostics)
		{
			AttachToParent();
			foreach (var diagnostic in diagnostics)
			{
				_topLevelDiagnostics.Add(diagnostic);
			}
		}

		public DiagnosticBag CreateChildBag()
		{
			var child = new DiagnosticBag(this);
			_childDiagnosticBags.Add(child);
			return child;
		}

		public IEnumerator<Diagnostic> GetEnumerator()
		{
			foreach (var diagnostic in _topLevelDiagnostics)
			{
				yield return diagnostic;
			}

			foreach (var diagnosticBag in _childDiagnosticBags)
			{
				foreach (var diagnostic in diagnosticBag)
				{
					yield return diagnostic;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

