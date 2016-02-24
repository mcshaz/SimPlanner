using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{    
    public class  DepartmentMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(256)]
        public string InvitationLetterFilename { get; set; }
        [StringLength(256)]
        public string CertificateFilename { get; set; }
    }
}
