using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.GlobalStore.Search;

namespace ConfTool.Client
{
    public partial class MainLayout
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;

        private string _searchTerm = string.Empty;

        private void StartSearch(string term)
        {
            _searchTerm = term;
            _dispatcher.Dispatch(new SetSearchTermAction(_searchTerm));
        }
    }
}