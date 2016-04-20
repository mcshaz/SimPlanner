using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseScenarioFacultyRoleMaps
    {
        internal static Func<CourseScenarioFacultyRoleDto, CourseScenarioFacultyRole> mapToRepo()
        { 
            return m => new CourseScenarioFacultyRole
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                FacultyScenarioRoleId =m.FacultyScenarioRoleId
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }

        internal static Expression<Func<CourseScenarioFacultyRole, CourseScenarioFacultyRoleDto>> mapFromRepo()
        {
            return m => new CourseScenarioFacultyRoleDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }
    }
}
