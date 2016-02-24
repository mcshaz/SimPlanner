using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CourseParticipantMaps
    {        internal static Func<CourseParticipantDto, CourseParticipant> mapToRepo = m => new CourseParticipant
        {
            ParticipantId = m.ParticipantId,
            CourseId = m.CourseId,
            IsConfirmed = m.IsConfirmed,
            IsFaculty = m.IsFaculty,
            DepartmentId = m.DepartmentId,
            ProfessionalRoleId = m.ProfessionalRoleId
            //Participant = m.Participant,
            //Course = m.Course
        };

        internal static Expression<Func<CourseParticipant, CourseParticipantDto>> mapFromRepo= m => new CourseParticipantDto
        {
            ParticipantId = m.ParticipantId,
            CourseId = m.CourseId,
            IsConfirmed = m.IsConfirmed,
            IsFaculty = m.IsFaculty,
            DepartmentId = m.DepartmentId,
            ProfessionalRoleId = m.ProfessionalRoleId
            //Participant = m.Participant,
            //Course = m.Course
        };
    }
}
