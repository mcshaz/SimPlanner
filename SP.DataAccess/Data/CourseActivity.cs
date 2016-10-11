using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(CourseActivityMetadata))]
    public class CourseActivity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid CourseTypeId { get; set; }

        public virtual CourseType CourseType { get; set; }


        public virtual ICollection<CourseSlot> CourseSlots { get; set; }


        public virtual ICollection<Activity> ActivityChoices { get; set; }
    }
}
