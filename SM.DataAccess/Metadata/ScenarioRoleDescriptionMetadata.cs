using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{
	public class FacultySimRoleMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Range(0,20)]
        public byte Order { get; set; }
    }
}
