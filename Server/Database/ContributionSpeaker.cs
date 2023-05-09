namespace ConfTool.Server.Database
{
    public class ContributionSpeaker
    {
        public Guid ContributionId { get; set; }
        public Guid SpeakerId { get; set; }

        public Contribution Contribution { get; set; } = default!;
        public Speaker Speaker { get; set; } = default!;
    }
}
