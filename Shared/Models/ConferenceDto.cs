using System.Runtime.Serialization;

namespace ConfTool.Shared.Models
{
    [DataContract]
    public class ConferenceDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }
        
        [DataMember(Order = 2)]
        public string Title { get; set; } = string.Empty;
        
        [DataMember(Order = 3)]
        public string Description { get; set; } = string.Empty;
        
        [DataMember(Order = 4)]
        public DateTime StartDate { get; set; }
        
        [DataMember(Order = 5)]
        public DateTime EndDate { get; set; }
    }
}
