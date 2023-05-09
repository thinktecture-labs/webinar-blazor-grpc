using Fluxor;

namespace ConfTool.Client.GlobalStore.Dialog
{
    [FeatureState]
    public record DialogState
    {
        public bool Visible { get; set; }
        public DialogContext? Context { get; set; } = default!;
    }
}
