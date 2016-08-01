using SP.DataAccess;namespace SP.Dto.Maps
{
    internal class CourseTypeDepartmentMaps: DomainDtoMap<CourseTypeDepartment, CourseTypeDepartmentDto>
    {
        public CourseTypeDepartmentMaps() : base(m => new CourseTypeDepartment
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            },
            m => new CourseTypeDepartmentDto
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            };
        }
    }
}
