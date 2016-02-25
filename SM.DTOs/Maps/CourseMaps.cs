using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq;
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

        internal static Expression<Func<Course, CourseDto>> mapFromRepo = m => new CourseDto
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            ParticipantVideoFilename = m.ParticipantVideoFilename,
            FeedbackSummaryFilename = m.FeedbackSummaryFilename,
            CourseTypeId = m.CourseTypeId,



            Scenarios = m.Scenarios.Select(s => new ScenarioDto
            {
                Id = s.Id,
                Description = s.Description,
                DepartmentId = s.DepartmentId,
                Complexity = s.Complexity,
                EmersionCategory = s.EmersionCategory,
                TemplateFilename = s.TemplateFilename,
                ManequinId = s.ManequinId,
                CourseTypeId = s.CourseTypeId,
            }).ToList()
            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };


        internal static Expression<Func<Course, CourseDto>> mapBriefFromRepo = m => new CourseDto
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            CourseTypeId = m.CourseTypeId,

            CourseParticipants = m.CourseParticipants.Select(cp=>new CourseParticipantDto
            {
                ParticipantId = cp.ParticipantId,
                CourseId = cp.CourseId,
                IsConfirmed = cp.IsConfirmed,
                IsFaculty = cp.IsFaculty,
                DepartmentId = cp.DepartmentId,
                ProfessionalRoleId = cp.ProfessionalRoleId
            }).ToList()

            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,


            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };

    }
}
