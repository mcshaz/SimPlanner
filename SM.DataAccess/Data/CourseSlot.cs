namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CourseSlot : Slot
    {

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public byte MinimumFaculty { get; set; }

        public byte MaximumFaculty { get; set; }

        ICollection<CourseTeachingResource> _defaultResources;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<CourseTeachingResource> DefaultResources
        {
            get
            {
                return _defaultResources ?? (_defaultResources = new List<CourseTeachingResource>());
            }
            set { _defaultResources = value; }
        }

    }
}
