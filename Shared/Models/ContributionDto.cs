using System.Runtime.Serialization;

namespace ConfTool.Shared.Models
{
    [DataContract]
    public class ContributionDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }
        
        [DataMember(Order = 2)]
        public string Title { get; set; } = string.Empty;
        
        [DataMember(Order = 3)]
        public string Abstract { get; set; } = string.Empty;
        
        [DataMember(Order = 4)]
        public string Url { get; set; } = string.Empty;
        
        [DataMember(Order = 5)]
        public DateTime StartDate { get; set; }
        
        [DataMember(Order = 6)]
        public DateTime EndDate { get; set; }
        
        [DataMember(Order = 7)]
        public IList<SpeakerDto> Speakers { get; set; } = new List<SpeakerDto>();
    }
}
