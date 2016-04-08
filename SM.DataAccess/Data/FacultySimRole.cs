namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [MetadataType(typeof(FacultySimRoleMetadata))]
    public class FacultySimRole
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid CourseTypeId { get; set; }
    
		public CourseType CourseType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }

    }
}
