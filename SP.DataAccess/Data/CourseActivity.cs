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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlot> CourseSlots { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityTeachingResource> ActivityChoices { get; set; }
    }
}
