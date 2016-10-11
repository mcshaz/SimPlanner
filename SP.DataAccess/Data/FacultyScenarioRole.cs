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


        public ICollection<CourseTypeScenarioRole> CourseTypeScenarioRoles { get; set; }


        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }

    }
}
