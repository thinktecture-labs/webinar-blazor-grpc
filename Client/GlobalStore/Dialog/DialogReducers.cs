using Fluxor;

namespace ConfTool.Client.GlobalStore.Dialog
{
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
