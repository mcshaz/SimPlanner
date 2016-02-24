using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SM.Dto
{
    [MetadataType(typeof(CourseMetadata))]
    public class CourseDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public byte FacultyNoRequired { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }
        public Guid CourseTypeId { get; set; }
        public DepartmentDto Department { get; set; }
        public DepartmentDto OutreachingDepartment { get; set; }
        public CourseTypeDto CourseType { get; set; }
        public ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public ICollection<ScenarioDto> Scenarios { get; set; }
        public ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

        internal static Func<CourseDto, Course> mapToRepo = m => new Course
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            ParticipantVideoFilename = m.ParticipantVideoFilename,
            FeedbackSummaryFilename = m.FeedbackSummaryFilename,
            CourseTypeId = m.CourseTypeId,
        };

        internal static Expression<Func<Course, CourseDto>> mapFromRepo= m => new CourseDto
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            ParticipantVideoFilename = m.ParticipantVideoFilename,
            FeedbackSummaryFilename = m.FeedbackSummaryFilename,
            CourseTypeId = m.CourseTypeId,

            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,

            //CourseParticipants = m.CourseParticipants,

            //Scenarios = m.Scenarios,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
    }
}
