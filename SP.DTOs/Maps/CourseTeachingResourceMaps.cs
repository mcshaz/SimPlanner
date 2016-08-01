using SP.DataAccess;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ActivityTeachingResourceMaps
    {
        internal static Func<ActivityTeachingResourceDto, ActivityTeachingResource> MapToDomain()
        { 
            return m => new ActivityTeachingResource {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId
            };
        }

        internal static Expression<Func<ActivityTeachingResource, ActivityTeachingResourceDto>> MapFromDomain()
        {
            return m => new ActivityTeachingResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId,
            };
        }
    }
}
