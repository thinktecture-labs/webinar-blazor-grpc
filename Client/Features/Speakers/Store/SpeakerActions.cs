using ConfTool.Shared.Models;

namespace ConfTool.Client.Features.Speakers.Store
{
    public class SpeakerActions
    {
        public record LoadSpeakersAction();
        public record LoadSpeakersActionSuccess(List<SpeakerDto> Speakers);
        public record LoadSpeakersActionFailed(string ErrorMessage);
    }
}
