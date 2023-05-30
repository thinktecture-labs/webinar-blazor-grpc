using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Client.State;

namespace ConfTool.Client.Features.Contributions.Store
{
    [FeatureState]
    public record ContributionState : FeatureStateBase<ContributionDto>
    {
        public List<SpeakerDto> Speakers { get; init; } = new();
    }
}
