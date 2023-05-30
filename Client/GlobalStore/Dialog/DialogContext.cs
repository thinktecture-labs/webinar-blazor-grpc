namespace ConfTool.Client.GlobalStore.Dialog
{
    public record DialogContext(Type ComponentType, Dictionary<string, object> Parameters, string DialogTitle);
}
