namespace SM.Dto
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseTypeScenarioRoleMetadata))]
    public partial class CourseTypeScenarioRoleDto
    {
        public Guid CourseTypeId { get; set; }

        public Guid FacultyScenarioRoleId { get; set; }

        public virtual CourseTypeDto CourseType { get; set; }

        public virtual FacultyScenarioRoleDto FacultyScenarioRole { get; set; }
    }
}
