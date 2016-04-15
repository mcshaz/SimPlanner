using SM.DataAccess;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseTypeDepartmentMaps
    {
        internal static Func<CourseTypeDepartmentDto, CourseTypeDepartment> mapToRepo()
        {
            return m => new CourseTypeDepartment
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            };
        }


        internal static Expression<Func<CourseTypeDepartment, CourseTypeDepartmentDto>> mapFromRepo()
        {
            return m => new CourseTypeDepartmentDto
            {
                CourseTypeId = m.CourseTypeId,
                DepartmentId = m.DepartmentId
            };
        }
    }
}
