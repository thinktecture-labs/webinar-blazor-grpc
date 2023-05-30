namespace ConfTool.Shared.Models
{
    public class ContributionDto
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Abstract { get; set; } = string.Empty;
        
        public string Url { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public IList<SpeakerDto> Speakers { get; set; } = new List<SpeakerDto>();
    }
}
