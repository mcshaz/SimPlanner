using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseTeachingResourceMaps
    {
        internal static Func<CourseTeachingResourceDto, CourseTeachingResource> mapToRepo()
        { 
            return m => new CourseTeachingResource {
                Id = m.Id,
                Name = m.Name,
                ResourceFilename = m.ResourceFilename,
                CourseSlotId = m.CourseSlotId,
                //CourseSlot = m.CourseSlot
            };
        }

        internal static Expression<Func<CourseTeachingResource, CourseTeachingResourceDto>> mapFromRepo()
        {
            return m => new CourseTeachingResourceDto
            {
                Id = m.Id,
                Name = m.Name,
                ResourceFilename = m.ResourceFilename,
                CourseSlotId = m.CourseSlotId,
                //CourseSlot = m.CourseSlot
            };
        }
    }
}
