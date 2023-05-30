using Fluxor;
using static ConfTool.Client.Features.Contributions.Store.ContributionActions;

namespace ConfTool.Client.Features.Contributions.Store
{
    public static class ContributionReducers
    {
        [ReducerMethod]
        public static ContributionState LoadContribution(ContributionState state, LoadContributionsAction _)
            => state with { LoadCollection = true };
        
        [ReducerMethod]
        public static ContributionState LoadContribution(ContributionState state, LoadContributionsActionSuccess action)
            => state with { LoadCollection = false, Collection = action.Contributions };

        [ReducerMethod]
        public static ContributionState LoadContribution(ContributionState state, LoadContributionsActionFailed action)
            => state with { LoadCollection = false, ErrorMessage = action.ErrorMessage };


        [ReducerMethod]
        public static ContributionState SetContribution(ContributionState state, SetEditContributionAction action)
        {
            var speakers = state.Collection.SelectMany(c => c.Speakers).DistinctBy(s => s.Id).ToList();
            return state with { LoadCollection = false, EditItem = action.Contribution, Speakers = speakers };
        }

        [ReducerMethod]
        public static ContributionState SaveContribution(ContributionState state, SaveContributionAction _)
            => state with { Saving = true };

        [ReducerMethod]
        public static ContributionState SaveContribution(ContributionState state, SaveContributionSuccessAction _)
            => state with { Saving = false };
    }
}
