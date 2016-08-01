using SP.DataAccess;namespace SP.Dto.Maps
{
    internal class ActivityTeachingResourceMaps: DomainDtoMap<ActivityTeachingResource, ActivityTeachingResourceDto>
        { 
            return m => new ActivityTeachingResource {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId
            },
            m => new ActivityTeachingResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                CourseActivityId = m.CourseActivityId,
            };
        }
    }
}
