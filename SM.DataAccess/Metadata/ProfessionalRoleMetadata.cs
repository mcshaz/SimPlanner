using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{    
    public class ProfessionalRoleMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        /// <summary>
        /// 1st 2 digits Professional category, second 2 order within category
        /// </summary>
        [Range(0,9999)]
        public int Order { get; set; }
    }
}
