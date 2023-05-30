using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfTool.Server.Database
{
    public class Speaker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public ICollection<ContributionSpeaker> ContributionSpeakers { get; set; } = new List<ContributionSpeaker>();
    }
}
