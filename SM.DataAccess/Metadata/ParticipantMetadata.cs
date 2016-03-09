using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{    
	public class ParticipantMetadata
    {
        [Phone]
        [StringLength(32)]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
        [EmailAddress]
        [StringLength(256)]
        public string AlternateEmail { get; set; }
        [Required]
        [StringLength(256)]
        public string FullName { get; set; }
    }
}
