using Fluxor;

namespace ConfTool.Client.State
{
    public record DialogContext(Type ComponentType, Dictionary<string, object> Parameters, string DialogTitle);


    [FeatureState]
    public record DialogState
    {
        public bool Visible { get; set; }
        public DialogContext? Context { get; set; } = default!;
    }

    public record OpenDialogAction(DialogContext Context);
    public record CloseDialogAction();

    public class DialogReducers
    {
        [ReducerMethod]
        public static DialogState OpenDialog(DialogState state, OpenDialogAction action)
            => state with { Visible = true, Context = action.Context };

        [ReducerMethod]
        public static DialogState CloseDialog(DialogState state, CloseDialogAction _)
            => state with { Visible = false, Context = null };
    }
}
