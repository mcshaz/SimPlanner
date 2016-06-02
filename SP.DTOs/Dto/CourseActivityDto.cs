using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(CourseActivityMetadata))]
    public class CourseActivityDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CourseTypeId { get; set; }

        public CourseTypeDto CourseType { get; set; }

        public ICollection<CourseSlotDto> CourseSlots { get; set; }

        public ICollection<ActivityTeachingResourceDto> ActivityChoices { get; set; }
    }
}
