using SP.DataAccess;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseTypeDepartmentMaps
    {
        internal static Func<CourseTypeDepartmentDto, CourseTypeDepartment> MapToDomain()
        {
            return m => new CourseTypeDepartment
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            };
        }


        internal static Expression<Func<CourseTypeDepartment, CourseTypeDepartmentDto>> MapFromDomain()
        {
            return m => new CourseTypeDepartmentDto
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            };
        }
    }
}
