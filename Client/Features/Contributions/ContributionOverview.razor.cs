using Fluxor;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using ProtoBuf.Grpc;
using ConfTool.Client.Features.Contributions.State;
using ConfTool.Client.State;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.JSInterop;

namespace ConfTool.Client.Features.Contributions
{
    public partial class ContributionOverview : IDisposable
    {
        [Inject] private IContributionService _contributionSerivce { get; set; } = default!;
        [Inject] private IDispatcher _dispatcher { get; set; } = default!;
        [Inject] private IState<ContributionState> _state { get; set; } = default!;
        [Inject] private IState<SearchState> _searchState { get; set; } = default!;
        [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;

        private bool _loading => _state.Value.LoadCollection;
        private string _errorMessage => _state.Value.ErrorMessage;
        private ICollection<ContributionDto> _contributions => _state.Value?.Collection ?? new List<ContributionDto>();

        private CancellationTokenSource? _cts;
        private Guid _updatedId = Guid.Empty;
        private IJSObjectReference _module;

        protected override async Task OnInitializedAsync()
        {
            _dispatcher.Dispatch(new LoadContributionsAction(_searchState.Value.SearchTerm));
            _dispatcher.ActionDispatched += DispatcherOnActionDispatched;
            await UpdateAvailable();
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

        private async Task UpdateAvailable()
        {
            _cts = new CancellationTokenSource();
            var options = new CallOptions(cancellationToken: _cts.Token);

            try
            {
                await foreach (var id in _contributionSerivce.UpdatedContributionAsync(new CallContext(options)))
                {
                    Console.WriteLine($"New id: {id}");
                    var newId = Guid.Parse(id);
                    _updatedId = newId;
                    StateHasChanged();
                    await ScrollElementIntoView(_updatedId);
                }
            }
            catch (RpcException)
            {
            }
            catch (OperationCanceledException)
            {
            }
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