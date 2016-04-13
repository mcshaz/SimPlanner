namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseFormatMetadata))]
    public partial class CourseFormat
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public byte DaysDuration { get; set; }
        public Guid CourseTypeId { get; set; }

        public virtual CourseType CourseType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlot> CourseSlots { get; set; }


    }
}
