using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ActivityTeachingResourceMaps
    {
        internal static Func<ActivityTeachingResourceDto, DataAccess.ActivityTeachingResource> mapToRepo()
        { 
            return m => new DataAccess.ActivityTeachingResource {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId,
                //CourseSlot = m.CourseSlot
            };
        }

        internal static Expression<Func<DataAccess.ActivityTeachingResource, ActivityTeachingResourceDto>> mapFromRepo()
        {
            return m => new ActivityTeachingResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId,
                //CourseSlot = m.CourseSlot
            };
        }
    }
}
