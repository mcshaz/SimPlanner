namespace SM.DataAccess
{
    using Enums;
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseTypeScenarioRoleMetadata))]
    public partial class CourseTypeScenarioRole
    {
        public Guid CourseTypeId { get; set; }

        public Guid FacultyScenarioRoleId { get; set; }

        public virtual CourseType CourseType { get; set; }

        public virtual FacultyScenarioRole FacultyScenarioRole { get; set; }
    }
}
