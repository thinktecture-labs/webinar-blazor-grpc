using Fluxor;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using ConfTool.Client.State;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;

namespace ConfTool.Client.Features.Conferences.State
{
    [FeatureState]
    public record ConferenceState : FeatureStateBase<ConferenceDto>;

    public record LoadConferencesAction(string SearchTerm);
    public record LoadConferencesActionSuccess(List<ConferenceDto> Conferences);
    public record LoadConferencesActionFailed(string ErrorMessage);
    public record SetEditConferenceAction(ConferenceDto Conference);
    public record SaveConferenceAction(ConferenceDto Conference);
    public record SaveConferenceSuccessAction();

    public static class ConferenceReducer
    {
        [ReducerMethod]
        public static ConferenceState LoadConference(ConferenceState state, LoadConferencesAction _)
            => state with { LoadCollection = true };

        [ReducerMethod]
        public static ConferenceState LoadConference(ConferenceState state, LoadConferencesActionSuccess action)
            => state with { LoadCollection = false, Collection = action.Conferences };

        [ReducerMethod]
        public static ConferenceState LoadConference(ConferenceState state, LoadConferencesActionFailed action)
            => state with { LoadCollection = false, ErrorMessage = action.ErrorMessage };


        [ReducerMethod]
        public static ConferenceState SetConference(ConferenceState state, SetEditConferenceAction action)
        {
            return state with { LoadCollection = false, EditItem = action.Conference };
        }

        [ReducerMethod]
        public static ConferenceState SaveConference(ConferenceState state, SaveConferenceAction _)
            => state with { Saving = true };

        [ReducerMethod]
        public static ConferenceState SaveConference(ConferenceState state, SaveConferenceSuccessAction _)
            => state with { Saving = false };
    }

    public class ConferencesEffect : Effect<LoadConferencesAction>
    {
        private readonly IConferencesService _conferencesService;

        public ConferencesEffect(IConferencesService conferencesService)
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
                    SearchTerm = action.SearchTerm
                });
                dispatcher.Dispatch(new LoadConferencesActionSuccess(result?.ToList() ?? new List<ConferenceDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadConferencesActionFailed($"Load Conferences failed. Error: {ex.Message}"));
            }
        }
    }

    public class SaveConferenceEffect : Effect<SaveConferenceAction>
    {
        private readonly IConferencesService _conferencesService;

        public SaveConferenceEffect(IConferencesService conferencesService)
        {
            _conferencesService = conferencesService ?? throw new ArgumentNullException(nameof(conferencesService));
        }

        public override async Task HandleAsync(SaveConferenceAction action, IDispatcher dispatcher)
        {
            try
            {
                await _conferencesService.AddOrUpdateConferenceAsync(new AddOrUpdateRequest<ConferenceDto>
                {
                    Id = action.Conference.Id,
                    Dto = action.Conference
                });
                dispatcher.Dispatch(new SaveConferenceSuccessAction());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Conferences failed. Error: {ex.Message}");
            }
        }
    }
}
