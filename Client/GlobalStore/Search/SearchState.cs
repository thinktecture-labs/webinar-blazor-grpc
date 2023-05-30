using Fluxor;

namespace ConfTool.Client.GlobalStore.Search
{
    [FeatureState]
    public record SearchState
    {
        public string SearchTerm { get; init; } = string.Empty;
    }
}
