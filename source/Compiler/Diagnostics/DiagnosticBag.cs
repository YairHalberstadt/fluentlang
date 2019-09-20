using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentLang.Compiler.Diagnostics
{
	public class DiagnosticBag : IEnumerable<Diagnostic>
	{
		private readonly ConcurrentBag<Diagnostic> _topLevelDiagnostics = new ConcurrentBag<Diagnostic>();
		private readonly ConcurrentBag<DiagnosticBag> _childDiagnosticBags = new ConcurrentBag<DiagnosticBag>();
		private readonly ISymbol _forSymbol;

		public DiagnosticBag(ISymbol forSymbol)
		{
			_forSymbol = forSymbol;
		}

		public void Add(Diagnostic diagnostic)
		{
			_topLevelDiagnostics.Add(diagnostic);
		}

		public void AddRange(IEnumerable<Diagnostic> diagnostics)
		{
			foreach (var diagnostic in diagnostics)
			{
				_topLevelDiagnostics.Add(diagnostic);
			}
		}

		public DiagnosticBag CreateChildBag(ISymbol forSymbol)
		{
			var child = new DiagnosticBag(forSymbol);
			_childDiagnosticBags.Add(child);
			return child;
		}

		public IEnumerator<Diagnostic> GetEnumerator()
		{
			// https://stackoverflow.com/a/20335369/7607408
			// this will be more efficient for large trees.

			var stack = new Stack<DiagnosticBag>();
			stack.Push(this);
			while (stack.Count > 0)
			{
				var current = stack.Pop();
				foreach (var diagnostic in current._topLevelDiagnostics)
					yield return diagnostic;
				foreach (var child in current._childDiagnosticBags)
					stack.Push(child);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private bool _allCollected;

		public void EnsureAllDiagnosticsCollectedForSymbol()
		{
			if (_allCollected)
				return;

			_forSymbol.EnsureAllLocalDiagnosticsCollected();

			foreach (var childBag in _childDiagnosticBags)
			{
				childBag.EnsureAllDiagnosticsCollectedForSymbol();
			}

			_allCollected = true;
		}
	}
}

