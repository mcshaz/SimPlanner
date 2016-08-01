using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseTypeMaps: DomainDtoMap<CourseType, CourseTypeDto>
    {
        public CourseTypeMaps() : base(m => new CourseType
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId, 
                EmersionCategory = m.EmersionCategory, 
            },
            m => new CourseTypeDto
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId,
                EmersionCategory = m.EmersionCategory,
                //Departments = m.Departments,

                //Scenarios = m.Scenarios,

                //Courses = m.Courses,

                //CourseEvents = m.CourseEvents,

                //ScenarioEvents = m.ScenarioEvents,

                //ScenarioRoles = m.ScenarioRoles
            })
        { }
    }
}
