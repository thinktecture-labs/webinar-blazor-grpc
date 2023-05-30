using Fluxor;

namespace ConfTool.Client.State
{
    [FeatureState]
    public record SearchState
    {
        public string SearchTerm { get; init; } = string.Empty;
    }

    public record SetSearchTermAction(string SearchTerm);

    public class SearchReducers
    {
        [ReducerMethod]
        public static SearchState SetSearchState(SearchState state, SetSearchTermAction action)
            => state with { SearchTerm = action.SearchTerm };
    }
}
