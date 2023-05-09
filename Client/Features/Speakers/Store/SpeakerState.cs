using Fluxor;
using ConfTool.Client.State;
using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Speakers.Store
{
    [FeatureState]
    public record SpeakerState : FeatureStateBase<SpeakerDto>;
}
