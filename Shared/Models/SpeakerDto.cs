using System.Runtime.Serialization;

namespace ConfTool.Shared.Models
{
    [DataContract]
    public class SpeakerDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }
        
        [DataMember(Order = 2)]
        public string FirstName { get; set; } = string.Empty;
        
        [DataMember(Order = 3)]
        public string LastName { get; set; } = string.Empty;
        
        [DataMember(Order = 4)]
        public string Abstract { get; set; } = string.Empty;
        
        [DataMember(Order = 5)]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
