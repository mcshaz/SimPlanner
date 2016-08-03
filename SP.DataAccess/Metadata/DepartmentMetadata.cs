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
        [Required]
        [StringLength(32)]
        public string Abbreviation { get; set; }
        [StringLength(256)]
        public string InvitationLetterFilename { get; set; }
        [StringLength(256)]
        public string CertificateFilename { get; set; }
        [FixedLength(Length = 6), RegularExpression(@"\d+"), Required, DefaultValue("000000")]
        public string PrimaryColour { get; set; }
        [FixedLength(Length = 6), RegularExpression(@"\d+"), Required, DefaultValue("000000")]
        public string SecondaryColour { get; set; }
    }
}
