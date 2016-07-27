using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{    
    public class  DepartmentMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(64)]
        public string Name { get; set; }
        [StringLength(16)]
        public string Abbreviation { get; set; }
        [StringLength(256)]
        public string InvitationLetterFilename { get; set; }
        [StringLength(256)]
        public string CertificateFilename { get; set; }
    }
}
