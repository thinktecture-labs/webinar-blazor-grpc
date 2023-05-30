using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using static ConfTool.Client.Features.Contributions.Store.ContributionActions;

namespace ConfTool.Client.Features.Contributions.Store
{
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
