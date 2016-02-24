namespace SM.DataAccess
{
    using SM.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlot : Slot
    {
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
