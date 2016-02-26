using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CourseMetadata))]
    public class CourseDto : BriefCourseDto
    {
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }

        public DepartmentDto Department { get; set; }
        public DepartmentDto OutreachingDepartment { get; set; }
        public CourseTypeDto CourseType { get; set; }

        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
        public virtual ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }
    }

    public class BriefCourseDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public byte FacultyNoRequired { get; set; }
        public Guid CourseTypeId { get; set; }

        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
    }
}
