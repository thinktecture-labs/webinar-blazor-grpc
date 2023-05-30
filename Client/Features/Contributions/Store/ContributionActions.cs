using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Contributions.Store
{
    public class ContributionActions
    {
        public record LoadContributionsAction(string SearchTerm);
        public record LoadContributionsActionSuccess(ICollection<ContributionDto> Contributions);
        public record LoadContributionsActionFailed(string ErrorMessage);
        public record SetEditContributionAction(ContributionDto? Contribution);
        public record SaveContributionAction(ContributionDto Contribution);
        public record SaveContributionSuccessAction();
    }
}
