namespace SP.DataAccess
{
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [MetadataType(typeof(FacultyScenarioRoleMetadata))]
    public class FacultyScenarioRole
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<CourseTypeScenarioRole> CourseTypeScenarioRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }

    }
}
