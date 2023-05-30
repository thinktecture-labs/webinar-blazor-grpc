using ConfTool.Client.Features.Speakers.Store;
using Fluxor;
using static ConfTool.Client.Features.Speakers.Store.SpeakerActions;

namespace ConfTool.Client.Features.Speakers.State
{
    public static class SpeakerReducers
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
    }
}
