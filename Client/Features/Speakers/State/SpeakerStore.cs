using Fluxor;
using System.Net.Http.Json;
using ConfTool.Client.State;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;

namespace ConfTool.Client.Features.Speakers.State
{
    [FeatureState]
    public record SpeakerState : FeatureStateBase<SpeakerDto>;

    public record LoadSpeakersAction(string SearchTerm);
    public record LoadSpeakersActionSuccess(List<SpeakerDto> Speakers);
    public record LoadSpeakersActionFailed(string ErrorMessage);
    public record SetEditSpeakerAction(SpeakerDto Speaker);
    public record SaveSpeakerAction(SpeakerDto Speaker);
    public record SaveSpeakerSuccessAction();

    public static class SpeakerReducer
    {
        [ReducerMethod]
        public static SpeakerState LoadSpeaker(SpeakerState state, LoadSpeakersAction _)
            => state with { LoadCollection = true };

        [ReducerMethod]
        public static SpeakerState LoadSpeaker(SpeakerState state, LoadSpeakersActionSuccess action)
            => state with { LoadCollection = false, Collection = action.Speakers };

        [ReducerMethod]
        public static SpeakerState LoadSpeaker(SpeakerState state, LoadSpeakersActionFailed action)
            => state with { LoadCollection = false, ErrorMessage = action.ErrorMessage };


        [ReducerMethod]
        public static SpeakerState SetSpeaker(SpeakerState state, SetEditSpeakerAction action)
        {
            return state with { LoadCollection = false, EditItem = action.Speaker };
        }

        [ReducerMethod]
        public static SpeakerState SaveSpeaker(SpeakerState state, SaveSpeakerAction _)
            => state with { Saving = true };

        [ReducerMethod]
        public static SpeakerState SaveSpeaker(SpeakerState state, SaveSpeakerSuccessAction _)
            => state with { Saving = false };
    }

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
                    SearchTerm = action.SearchTerm,
                    Skip = 0,
                    Take = 100
                });
                dispatcher.Dispatch(new LoadSpeakersActionSuccess(result?.ToList() ?? new List<SpeakerDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadSpeakersActionFailed($"Load Speakers failed. Error: {ex.Message}"));
            }
        }
    }

    public class SaveSpeakerEffect : Effect<SaveSpeakerAction>
    {
        private readonly ISpeakersService _speakersService;

        public SaveSpeakerEffect(ISpeakersService speakersService)
        {
            _speakersService = speakersService ?? throw new ArgumentNullException(nameof(speakersService));
        }

        public override async Task HandleAsync(SaveSpeakerAction action, IDispatcher dispatcher)
        {
            try
            {
                await _speakersService.AddOrUpdateSpeakerAsync(new AddOrUpdateRequest<SpeakerDto>
                {
                    Id = action.Speaker.Id,
                    Dto = action.Speaker
                });
                dispatcher.Dispatch(new SaveSpeakerSuccessAction());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Speakers failed. Error: {ex.Message}");
            }
        }
    }
}
