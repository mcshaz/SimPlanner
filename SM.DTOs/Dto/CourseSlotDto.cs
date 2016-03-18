using SM.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlotDto : SlotDto
    {
        public string Name { get; set; }
        public byte MinimumFaculty { get; set; }
        public byte MaximumFaculty { get; set; }
        public virtual ICollection<CourseTeachingResourceDto> DefaultResources { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
    }
}
