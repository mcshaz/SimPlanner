using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ActivityTeachingResourceMaps: DomainDtoMap<ActivityTeachingResource, ActivityTeachingResourceDto>
    { 
        public ActivityTeachingResourceMaps() : base(
        m => new ActivityTeachingResource {
            Id = m.Id,
            Description = m.Description,
            FileName = m.FileName,
            CourseActivityId = m.CourseActivityId,
            FileModified = m.FileModified,
            FileSize = m.FileSize
        },
        m => new ActivityTeachingResourceDto
        {
            Id = m.Id,
            Description = m.Description,
            FileName = m.FileName,
            CourseActivityId = m.CourseActivityId,
            FileModified = m.FileModified,
            FileSize = m.FileSize
        })
        { }
    }
}
