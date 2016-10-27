namespace SP.Dto
{
    using DataAccess.Enums;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseTypeMetadata))]
    public partial class CourseTypeDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Abbreviation { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public Guid? InstructorCourseId { get; set; }

        public virtual CourseTypeDto InstructorCourse { get; set; }

        public ICollection<CourseActivityDto> CourseActivities { get; set; }

        public ICollection<CourseTypeDepartmentDto> CourseTypeDepartments { get; set; }

        public ICollection<ScenarioDto> Scenarios { get; set; }

        public ICollection<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles { get; set; }

        public ICollection<CourseFormatDto> CourseFormats { get; set; }

        public virtual ICollection<CandidatePrereadingDto> CandidatePrereadings { get; set; }
    }
}
