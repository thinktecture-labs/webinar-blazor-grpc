namespace ConfTool.Shared.Models
{
    public class ConferenceDto
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
    }
}
