using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.Features.Contributions.State;
using ConfTool.Client.State;
using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Contributions
{
    public class SelectableSpeaker {
        public int Id { get; set; }
        public bool Selected { get; set; }
        public SpeakerDto Speaker { get; set; }
        public SelectableSpeaker(int id, bool selected, SpeakerDto speaker)
        {
            Id = id;
            Selected = selected;
            Speaker = speaker;
        }
    }
    public partial class ContributionEditor : IDisposable
    {
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<ContributionState> _state { get; set; } = default!;

        private ContributionDto? _contributionDto;
        private List<SelectableSpeaker> _speakers = new ();

        protected override Task OnInitializedAsync()
        {
            _dispatcher.ActionDispatched += _dispatcher_ActionDispatched;
            if (_state.Value.EditItem is null)
            {
                _contributionDto = new ContributionDto() { Id = Guid.NewGuid(), StartDate = DateTime.Now, EndDate = DateTime.Now, Speakers = new List<SpeakerDto>() };
                _speakers = _state.Value.Speakers.Select((s, index) => new SelectableSpeaker(index, false, s)).ToList();
            }
            else
            {
                _contributionDto = _state.Value.EditItem;
                _speakers = _state.Value.Speakers.Select((s, index) => new SelectableSpeaker(index, _contributionDto.Speakers.Any(sp => s.Id == sp.Id), s)).ToList();
            }
            return base.OnInitializedAsync();
        }

        private void _dispatcher_ActionDispatched(object? sender, ActionDispatchedEventArgs e)
        {
            if (e.Action.GetType() == typeof(SaveContributionSuccessAction))
            {
                _dispatcher.Dispatch(new CloseDialogAction());
            }
        }

        private void SelectSpeaker(int id)
        {
            var speaker = _speakers.FirstOrDefault(s => s.Id == id);
            if (speaker is not null)
            {
                speaker.Selected = !speaker.Selected;
            }
        }

        private void Cancel()
        {
            _dispatcher.Dispatch(new CloseDialogAction());
        }

        private void Save()
        {
            if (_contributionDto is not null)
            {
                _contributionDto.Speakers = _speakers
                    .Where(s => s.Selected)
                    .Select(s => s.Speaker)
                    .ToList();
                _dispatcher.Dispatch(new SaveContributionAction(_contributionDto));
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            _dispatcher.Dispatch(new SetEditContributionAction(null));
            _dispatcher.ActionDispatched -= _dispatcher_ActionDispatched;
        }
    }
}