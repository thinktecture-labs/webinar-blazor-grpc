using Fluxor;
using System.Net.Http.Json;
using ConfTool.Client.State;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;

namespace ConfTool.Client.Features.Contributions.State
{
    [FeatureState]
    public record ContributionState : FeatureStateBase<ContributionDto>
    {
        public List<SpeakerDto> Speakers { get; init; } = new();
    }

    public record LoadContributionsAction(string SearchTerm);
    public record LoadContributionsActionSuccess(ICollection<ContributionDto> Contributions);
    public record LoadContributionsActionFailed(string ErrorMessage);
    public record SetEditContributionAction(ContributionDto? Contribution);
    public record SaveContributionAction(ContributionDto Contribution);
    public record SaveContributionSuccessAction();

    public static class ContributionReducer
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

    public class ContributionsEffect : Effect<LoadContributionsAction>
    {
        private readonly IContributionService _contributionService;

        public ContributionsEffect(IContributionService contributionService)
        {
            _contributionService = contributionService;
        }

        public override async Task HandleAsync(LoadContributionsAction action, IDispatcher dispatcher)
        {
            try 
            {
                var result = await _contributionService.GetContributionsAsync(new CollectionRequest
                {
                   SearchTerm = action.SearchTerm,
                   Skip = 0,
                   Take = 100
                } );
                
                dispatcher.Dispatch(new LoadContributionsActionSuccess(result));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadContributionsActionFailed($"Load Contributions failed. Error: {ex.Message}"));
            }
        }
    }

    public class SaveContributionEffect : Effect<SaveContributionAction>
    {
        private readonly IContributionService _contributionService;

        public SaveContributionEffect(IContributionService contributionService)
        {
            _contributionService = contributionService ?? throw new ArgumentNullException(nameof(contributionService));
                    }

        public override async Task HandleAsync(SaveContributionAction action, IDispatcher dispatcher)
        {
            try
            {
                Console.WriteLine($"Speakers: {action.Contribution.Speakers.Count}");
                await _contributionService.AddOrUpdateContributionAsync(new AddOrUpdateRequest<ContributionDto>
                    {
                        Dto = action.Contribution,
                        Id = action.Contribution.Id
                    });
                dispatcher.Dispatch(new SaveContributionSuccessAction());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Contributions failed. Error: {ex.Message}");
            }
        }
    }
}
