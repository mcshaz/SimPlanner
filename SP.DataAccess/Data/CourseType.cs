namespace SP.DataAccess
{
    using Enums;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseTypeMetadata))]
    public partial class CourseType
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public Emersion? EmersionCategory { get; set; }

        public Guid? InstructorCourseId { get; set; }

        public virtual CourseType InstructorCourse { get; set; }

        public virtual ICollection<CourseActivity> CourseActivities { get; set; }

        public virtual ICollection<CourseTypeDepartment> CourseTypeDepartments { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public virtual ICollection<CourseTypeScenarioRole> CourseTypeScenarioRoles { get; set;}

        public virtual ICollection<CourseFormat> CourseFormats { get; set; }

        public virtual ICollection<CandidatePrereading> CandidatePrereading { get; set; }
    }
}
