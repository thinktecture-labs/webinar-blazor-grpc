using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Client.State;

namespace ConfTool.Client.Features.Conferences.Store
{
    [FeatureState]
    public record ConferenceState : FeatureStateBase<ConferenceDto>;
}
