using FluentLang.Compiler.Emit;
using FluentLang.Runtime;
using FluentLang.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace FluentLang.WebIde.Backend
{
	public class WasmMetadataReferenceProvider : IMetadataReferenceProvider, IRequiresAsyncInitialize
	{
		private readonly HttpClient _httpClient;
		private readonly NavigationManager _navigationManager;

		public WasmMetadataReferenceProvider(HttpClient httpClient, NavigationManager navigationManager)
		{
			_httpClient = httpClient;
			_navigationManager = navigationManager;
		}

		public ImmutableArray<MetadataReference> MetadataReferences { get; private set; }

		public async ValueTask InitializeAsync()
		{
			var baseAddress = new Uri(_navigationManager.BaseUri);

			var references = ImmutableArray.CreateBuilder<MetadataReference>();

			MetadataReferences = await _availableAssemblies.Distinct().ToAsyncEnumerable().SelectAwait(async x =>
			{
				var stream = await _httpClient.GetStreamAsync(new Uri(baseAddress, $"_framework/_bin/{x}"));
				return MetadataReference.CreateFromStream(stream);
			}).ToImmutableArrayAsync<MetadataReference>();
		}

		private static readonly string[] _availableAssemblies =
		{
			typeof(FLObject).Assembly.Location,
			typeof(ImmutableArray).Assembly.Location,
			typeof(AssemblyFileVersionAttribute).Assembly.Location,
			typeof(object).Assembly.Location,
			typeof(Span<>).Assembly.Location,
			typeof(Func<>).Assembly.Location,
			typeof(Attribute).Assembly.Location,
			"netstandard.dll",
		};
	}
}
