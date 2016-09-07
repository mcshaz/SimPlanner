using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto.Metadata
{
    public class RoleDtoMetadata 
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(256), Required]
        public string Name { get; set; }
        /*
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
        [Phone]
        [StringLength(128)]
        public string PhoneNumber { get; set; }
        */
    }
}
