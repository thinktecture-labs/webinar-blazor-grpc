using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;

namespace ConfTool.Client.Features.Conferences.Store
{
    public class LoadConferencesEffect : Effect<LoadConferencesAction>
    {
        private readonly IConferencesService _conferencesService;

        public LoadConferencesEffect(IConferencesService conferencesService)
        {
            _conferencesService = conferencesService;
        }

        public override async Task HandleAsync(LoadConferencesAction action, IDispatcher dispatcher)
        {
            try
            {
                var result = await _conferencesService.GetConferencesAsync(new CollectionRequest
                {
                    Skip = 0,
                    Take = 100,
                    SearchTerm = string.Empty
                });
                dispatcher.Dispatch(new LoadConferencesActionSuccess(result?.ToList() ?? new List<ConferenceDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadConferencesActionFailed($"Load Conferences failed. Error: {ex.Message}"));
            }
        }
    }
}
