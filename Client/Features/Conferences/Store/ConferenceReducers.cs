using Fluxor;

namespace ConfTool.Client.Features.Conferences.Store
{
    public static class ConferenceReducers
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
    }
}
