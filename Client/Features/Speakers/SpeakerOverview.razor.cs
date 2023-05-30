using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.Features.Speakers.State;
using ConfTool.Shared.Models;

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
            _dispatcher.Dispatch(new LoadSpeakersAction(string.Empty));
            _dispatcher.ActionDispatched += _dispatcher_ActionDispatched;
            await base.OnInitializedAsync();
        }

        private void _dispatcher_ActionDispatched(object? sender, ActionDispatchedEventArgs e)
        {
            if (e.Action.GetType() == typeof(SaveSpeakerSuccessAction))
            {
                _dispatcher.Dispatch(new LoadSpeakersAction(string.Empty));
            }
        }

        private void OpenEditor(SpeakerDto speaker)
        {
            //Console.WriteLine("Clicked speaker");
            //_dispatcher.Dispatch(new SetEditSpeakerAction(speaker));
            //_dispatcher.Dispatch(new OpenDialogAction(new DialogContext(typeof(SpeakerEditor), new Dictionary<string, object>
            //{
            //}, speaker.Title)));
        }

        public new void Dispose()
        {
            base.Dispose();
            _dispatcher.ActionDispatched -= _dispatcher_ActionDispatched;
        }
    }
}