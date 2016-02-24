using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CourseMaps
    {
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
