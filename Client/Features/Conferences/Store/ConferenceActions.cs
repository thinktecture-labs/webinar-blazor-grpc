using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Conferences.Store
{
    public record LoadConferencesAction();
    public record LoadConferencesActionSuccess(List<ConferenceDto> Conferences);
    public record LoadConferencesActionFailed(string ErrorMessage);
}
