using FluentLang.Compiler.Model;
using FluentLang.Compiler.Model.Diagnostic;
using FluentLang.Compiler.SemanticPhase;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.SemanticPhase
{
	public class TestCompilation : ISemanticModel
	{
		private readonly ISemanticModel _semanticModel;
		private readonly ImmutableList<Diagnostic> _diagnostics;

		public TestCompilation(ISemanticModel semanticModel, ImmutableList<Diagnostic> diagnostics)
		{
			_semanticModel = semanticModel;
			_diagnostics = diagnostics;
		}

		public bool TryGetInterface(QualifiedName fullyQualifiedName, out IInterface i)
		{
			return _semanticModel.TryGetInterface(fullyQualifiedName, out i);
		}

		public ISemanticModel? TryWith(IInterface i)
		{
			throw new System.NotImplementedException();
		}

		public ISemanticModel? TryWith(Method m)
		{
			throw new System.NotImplementedException();
		}

		public void VerifyDiagnosics(params Diagnostic[] diagnostics)
		{
			Assert.False(diagnostics.Select((x, i) =>
			{
				var y = _diagnostics[i];
				return x.ErrorCode == y.ErrorCode;
			}).Where(x => !x).Any(),
				$@"expected: 
{string.Concat("\n", diagnostics.Select(x => "new Diagnostic (new Location (), " + x.ErrorCode.ToString() + ")"))}

actual:
{string.Concat("\n", _diagnostics.Select(x => "new Diagnostic (new Location (), " + x.ErrorCode.ToString() + ")"))}
");
		}

		public ISemanticModel With(IInterface i)
		{
			throw new System.NotImplementedException();
		}
	}
}