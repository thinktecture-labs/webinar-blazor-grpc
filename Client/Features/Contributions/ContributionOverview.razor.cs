using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.JSInterop;
using ConfTool.Client.Features.Contributions.Store;
using ConfTool.Client.GlobalStore.Search;
using ConfTool.Client.GlobalStore.Dialog;
using static ConfTool.Client.Features.Contributions.Store.ContributionActions;

namespace ConfTool.Client.Features.Contributions
{
    public partial class ContributionOverview : IDisposable
    {
        [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<ContributionState> _state { get; set; } = default!;
        [Inject] private IState<SearchState> _searchState { get; set; } = default!;

        private bool _loading => _state.Value.LoadCollection;
        private string _errorMessage => _state.Value.ErrorMessage;
        private ICollection<ContributionDto> _contributions => _state.Value?.Collection ?? new List<ContributionDto>();

        private CancellationTokenSource? _cts;
        private Guid _updatedId = Guid.Empty;
        private IJSObjectReference? _module;

        protected override async Task OnInitializedAsync()
        {
            _dispatcher.Dispatch(new LoadContributionsAction(_searchState.Value.SearchTerm));
            _dispatcher.ActionDispatched += DispatcherOnActionDispatched;
            await base.OnInitializedAsync();
        }

        private void DispatcherOnActionDispatched(object? sender, ActionDispatchedEventArgs e)
        {
            if (e.Action.GetType() == typeof(SaveContributionSuccessAction)
                || e.Action.GetType() == typeof(SetSearchTermAction))
            {
                _dispatcher.Dispatch(new LoadContributionsAction(_searchState.Value.SearchTerm));
            }
        }

        private void OpenEditor(ContributionDto contribution)
        {
            Console.WriteLine("Clicked contribution");
            _dispatcher.Dispatch(new SetEditContributionAction(contribution));
            _dispatcher.Dispatch(new OpenDialogAction(new DialogContext(typeof(ContributionEditor),
                new Dictionary<string, object>
                {
                }, contribution.Title)));
        }

        

        private async Task ScrollElementIntoView(Guid id)
        {
            if (_module is null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Features/Contributions/ContributionOverview.razor.js");
            }

            await _module.InvokeVoidAsync("scrollToElement", id);
        }

        public new void Dispose()
        {
            _dispatcher.ActionDispatched -= DispatcherOnActionDispatched;
            _cts?.Cancel();
            base.Dispose();
        }
    }
}