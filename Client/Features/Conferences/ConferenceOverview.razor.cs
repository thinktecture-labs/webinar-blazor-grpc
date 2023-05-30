using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.Features.Conferences.State;
using ConfTool.Client.State;
using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Conferences
{
    public partial class ConferenceOverview
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<ConferenceState> _state { get; set; } = default!;
        [Inject] private IState<SearchState> _searchState { get; set; } = default!;

        private bool _loading => _state.Value.LoadCollection;
        private string _errorMessage => _state.Value.ErrorMessage;
        private ICollection<ConferenceDto> _conferences => _state.Value.Collection;

        protected override async Task OnInitializedAsync()
        {
            _dispatcher.Dispatch(new LoadConferencesAction(_searchState.Value.SearchTerm));
            _dispatcher.ActionDispatched += _dispatcher_ActionDispatched;
            await base.OnInitializedAsync();
        }

        private void _dispatcher_ActionDispatched(object? sender, ActionDispatchedEventArgs e)
        {
            if (e.Action.GetType() == typeof(SaveConferenceSuccessAction)
                || e.Action.GetType() == typeof(SetSearchTermAction))
            {
                _dispatcher.Dispatch(new LoadConferencesAction(_searchState.Value.SearchTerm));
            }
        }

        private void OpenEditor(ConferenceDto conference)
        {
            //Console.WriteLine("Clicked conference");
            //_dispatcher.Dispatch(new SetEditConferenceAction(conference));
            //_dispatcher.Dispatch(new OpenDialogAction(new DialogContext(typeof(ConferenceEditor), new Dictionary<string, object>
            //{
            //}, conference.Title)));
        }

        public new void Dispose()
        {
            base.Dispose();
            _dispatcher.ActionDispatched -= _dispatcher_ActionDispatched;
        }
    }
}