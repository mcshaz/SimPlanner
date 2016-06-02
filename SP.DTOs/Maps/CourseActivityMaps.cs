using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseActivityMaps
    {
        internal static Func<CourseActivityDto, CourseActivity> mapToRepo()
        {
            return m => new CourseActivity
            {
                Id = m.Id,
                Name = m.Name,
                CourseTypeId = m.CourseTypeId
            };
        }

        internal static Expression<Func<CourseActivity, CourseActivityDto>> mapFromRepo()
        {
            return m => new CourseActivityDto
            {
                Id = m.Id,
                Name = m.Name,
                CourseTypeId = m.CourseTypeId
            };
        }
    }
}
