using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ChosenTeachingResourceMaps : DomainDtoMap<ChosenTeachingResource, ChosenTeachingResourceDto>
    {
        public ChosenTeachingResourceMaps() : base(m => new ChosenTeachingResource
                    {
                        CourseId = m.CourseId,
                        CourseSlotId = m.CourseSlotId,
                        ActivityTeachingResourceId = m.ActivityTeachingResourceId
                    },
                    m => new ChosenTeachingResourceDto
                    {
                        CourseId = m.CourseId,
                        CourseSlotId = m.CourseSlotId,
                        ActivityTeachingResourceId = m.ActivityTeachingResourceId
                    })
        {
        }
    }
}
