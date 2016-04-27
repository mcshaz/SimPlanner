using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseSlotScenarioMaps
    {
        internal static Func<CourseSlotScenarioDto, CourseSlotScenario> mapToRepo()
        { 
            return m => new CourseSlotScenario
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }

        internal static Expression<Func<CourseSlotScenario, CourseSlotScenarioDto>> mapFromRepo()
        {
            return m => new CourseSlotScenarioDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }
    }
}
