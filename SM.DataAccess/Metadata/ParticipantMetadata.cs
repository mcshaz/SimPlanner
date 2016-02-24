using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{    
	public class ParticipantMetadata
    {
        [EmailAddress]
        [StringLength(256)]
        public string AlternateEmail { get; set; }
        [StringLength(256)]
        public string FullName { get; set; }
    }
}
