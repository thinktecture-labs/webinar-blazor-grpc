using Fluxor;

namespace ConfTool.Client.GlobalStore.Search
{
    public class SearchReducers
    {
        [ReducerMethod]
        public static SearchState SetSearchState(SearchState state, SetSearchTermAction action)
            => state with { SearchTerm = action.SearchTerm };
    }
}
