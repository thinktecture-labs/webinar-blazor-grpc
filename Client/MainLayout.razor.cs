using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.State;

namespace ConfTool.Client
{
    public partial class MainLayout
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;

        private string _searchTerm = string.Empty;

        private void StartSearch()
        {
            _dispatcher.Dispatch(new SetSearchTermAction(_searchTerm));
        }
    }
}