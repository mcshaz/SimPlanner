using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(CourseActivityMetadata))]
    public class CourseActivity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid CourseTypeId { get; set; }

        public CourseType CourseType { get; set; }

        ICollection<CourseSlot> _courseSlots;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlot> CourseSlots
        {
            get
            {
                return _courseSlots ?? (_courseSlots = new List<CourseSlot>());
            }
            set
            {
                _courseSlots = value;
            }
        }

        ICollection<ActivityTeachingResource> _activityChoices;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityTeachingResource> ActivityChoices
        {
            get
            {
                return _activityChoices ?? (_activityChoices = new List<ActivityTeachingResource>());
            }
            set
            {
                _activityChoices = value;
            }
        }
    }
}
