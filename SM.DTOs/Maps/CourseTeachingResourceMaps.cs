using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class ActivityTeachingResourceMaps
    {
        internal static Func<ActivityTeachingResourceDto, DataAccess.ActivityTeachingResource> mapToRepo()
        { 
            return m => new DataAccess.ActivityTeachingResource {
                Id = m.Id,
                Name = m.Name,
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
                Name = m.Name,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId,
                //CourseSlot = m.CourseSlot
            };
        }
    }
}
