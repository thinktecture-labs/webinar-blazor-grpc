using Microsoft.AspNetCore.Components;

namespace ConfTool.Client.Components
{
    public enum ButtonAppearance
    {
        Fill,
        Outline,
        None,
    }

    public partial class Button
    {
        [Parameter] public ButtonAppearance Appearance { get; set; } = ButtonAppearance.Fill;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public EventCallback OnClick { get; set; } = default!;

        private string _appearanceClass = string.Empty;

        protected override void OnInitialized()
        {
            _appearanceClass = Appearance switch
            {
                ButtonAppearance.Outline => "outline",
                ButtonAppearance.None => "none",
                _ => "fill",
            };
            base.OnInitialized();
        }

        private async Task Clicked() => await OnClick.InvokeAsync();
    }
}