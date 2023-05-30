using Fluxor;
using Microsoft.AspNetCore.Components;
using ConfTool.Client.State;

namespace ConfTool.Client.Components
{
    public partial class Dialog : IDisposable
    {
        [Inject] IState<DialogState> _state { get; set; } = default!;
        [Inject] IDispatcher _dispatcher { get; set; } = default!;


        public bool _showModal = false;
        private string _title = string.Empty;
        private RenderFragment? _dialogContent;

        protected override void OnInitialized()
        {
            _dispatcher.ActionDispatched += _dispatcher_ActionDispatched;

        }

        private void _dispatcher_ActionDispatched(object? sender, ActionDispatchedEventArgs e)
        {
            if (e.Action.GetType() == typeof(OpenDialogAction))
            {
                if (_state.Value.Context is not null)
                {
                    ShowDialog(_state.Value.Context);
                }
            }
            else if (e.Action.GetType() == typeof(CloseDialogAction))
            {
                _showModal = false;
                StateHasChanged();
            }

        }

        private void ShowDialog(DialogContext context)
        {
            _title = context.DialogTitle;
            _dialogContent = __builder =>
            {
                __builder.OpenComponent(0, context.ComponentType);
                if (context.Parameters?.Count > 0)
                {
                    foreach (var param in context.Parameters)
                    {
                        __builder.AddAttribute(1, param.Key, param.Value);
                    }
                }
                __builder.CloseComponent();
            };
            _showModal = true;
            StateHasChanged();
        }

        private void CloseDialog()
        {
            _dispatcher.Dispatch(new CloseDialogAction());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _dispatcher.ActionDispatched -= _dispatcher_ActionDispatched;
        }
    }
}