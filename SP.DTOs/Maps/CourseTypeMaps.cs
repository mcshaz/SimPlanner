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
            })
        { }
    }
}
