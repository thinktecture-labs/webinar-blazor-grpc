using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.Features.Speakers.State;
using ConfTool.Shared.Models;
using static ConfTool.Client.Features.Speakers.Store.SpeakerActions;
using ConfTool.Client.Features.Speakers.Store;

namespace ConfTool.Client.Features.Speakers
{
    public partial class SpeakerOverview : IDisposable
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<SpeakerState> _state { get; set; } = default!;

        private bool _loading => _state.Value.LoadCollection;
        private string _errorMessage => _state.Value.ErrorMessage;
        private ICollection<SpeakerDto> _speakers => _state.Value.Collection;

        protected override async Task OnInitializedAsync()
        {
            _dispatcher.Dispatch(new LoadSpeakersAction());
            await base.OnInitializedAsync();
        }
    }
}