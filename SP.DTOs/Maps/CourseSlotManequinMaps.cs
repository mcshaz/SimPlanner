using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseSlotManequinMaps
    {
        internal static Func<CourseSlotManequinDto, CourseSlotManequin> mapToRepo()
        { 
            return m => new CourseSlotManequin
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManequinId = m.ManequinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }

        internal static Expression<Func<CourseSlotManequin, CourseSlotManequinDto>> mapFromRepo()
        {
            return m => new CourseSlotManequinDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManequinId = m.ManequinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }
    }
}
