using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using static ConfTool.Client.Features.Speakers.Store.SpeakerActions;

namespace ConfTool.Client.Features.Speakers.State
{
    public class SpeakersEffect : Effect<LoadSpeakersAction>
    {
        private readonly ISpeakersService _speakersService;

        public SpeakersEffect(ISpeakersService speakersService)
        {
            _speakersService = speakersService ?? throw new ArgumentNullException(nameof(speakersService));
        }

        public override async Task HandleAsync(LoadSpeakersAction action, IDispatcher dispatcher)
        {
            try
            {
                var result = await _speakersService.GetSpeakersAsync(new CollectionRequest
                {
                    Skip = 0,
                    Take = 100,
                    SearchTerm = string.Empty
                });
                dispatcher.Dispatch(new LoadSpeakersActionSuccess(result?.ToList() ?? new List<SpeakerDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadSpeakersActionFailed($"Load Speakers failed. Error: {ex.Message}"));
            }
        }
    }
}
