using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Shared.Models;
using ConfTool.Client.Features.Conferences.Store;

namespace ConfTool.Client.Features.Conferences
{
    public partial class ConferenceOverview
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<ConferenceState> _state { get; set; } = default!;

        private bool _loading => _state.Value.LoadCollection;
        private string _errorMessage => _state.Value.ErrorMessage;
        private ICollection<ConferenceDto> _conferences => _state.Value.Collection;

        protected override async Task OnInitializedAsync()
        {
            _dispatcher.Dispatch(new LoadConferencesAction());
            await base.OnInitializedAsync();
        }
    }
}