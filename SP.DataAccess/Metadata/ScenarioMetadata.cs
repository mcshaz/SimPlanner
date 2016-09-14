using SP.DataAccess.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
    public class ScenarioMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(64)]
        public string BriefDescription { get; set; }
        [StringLength(256)]
        public string FullDescription { get; set; }
        [DefaultValue((int)SharingLevel.DepartmentOnly)]
        public SharingLevel Access { get; set; }
    }
}
