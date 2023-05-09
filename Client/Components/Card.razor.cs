using System.Text;
using Microsoft.AspNetCore.Components;

namespace ConfTool.Client.Components
{
    public partial class Card
    {
        [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;
        [Parameter, EditorRequired] public string Description { get; set; } = string.Empty;
        [Parameter] public string Id { get; set; } = Guid.NewGuid().ToString();
        [Parameter] public string CssClass { get; set; } = string.Empty;
        [Parameter] public bool Active { get; set; }
        [Parameter] public EventCallback Clicked { get; set; }
        [Parameter] public RenderFragment? HeaderContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private string _cssClass = "card";
        private bool _shouldRender = false;
        private int _currentHash = -1;
        protected override bool ShouldRender() => _shouldRender;


        protected override void OnParametersSet()
        {
            _cssClass = string.Join(" ", "card", Active ? "active" : string.Empty, CssClass);
            var hash = Title.GetHashCode() + Description.GetHashCode() + Active.GetHashCode();
            _shouldRender = hash != _currentHash;
            _currentHash = hash;
            base.OnParametersSet();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                Console.WriteLine($"Render card: {Title}");
            }
            base.OnAfterRender(firstRender);
        }

        private async Task CardClicked()
        {
            await Clicked.InvokeAsync(EventArgs.Empty);
        }
    }
}