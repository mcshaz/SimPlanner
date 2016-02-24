using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{    
	public class CourseSlotMetadata : SlotMetadata
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
