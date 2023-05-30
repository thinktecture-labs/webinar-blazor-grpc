namespace ConfTool.Shared.Models
{
    public class SpeakerDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Abstract { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
    }
}
