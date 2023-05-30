using Fluxor;
using ConfTool.Shared.Models;

namespace ConfTool.Client.State
{
    public record FeatureStateBase<T>
    {
        public bool LoadCollection { get; init; }
        public bool Saving { get; init; }
        public T? EditItem { get; set; }
        public ICollection<T> Collection { get; init; } = new List<T>();
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
