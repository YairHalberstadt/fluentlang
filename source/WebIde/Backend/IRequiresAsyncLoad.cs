using System.Threading.Tasks;

namespace FluentLang.WebIde.Backend
{
	public interface IRequiresAsyncInitialize
	{
		public ValueTask InitializeAsync();
	}
}