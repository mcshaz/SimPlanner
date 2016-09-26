using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ActivityMaps: DomainDtoMap<Activity, ActivityDto>
    { 
        public ActivityMaps() : base(
        m => new Activity {
            Id = m.Id,
            Description = m.Description,
            FileName = m.FileName,
            CourseActivityId = m.CourseActivityId,
            FileModified = m.FileModified,
            FileSize = m.FileSize
        },
        m => new ActivityDto
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
