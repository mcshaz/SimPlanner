using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseSlotPresenterMaps: DomainDtoMap<CourseSlotPresenter, CourseSlotPresenterDto>
    {
        public CourseSlotPresenterMaps() : base(m => new CourseSlotPresenter
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                StreamNumber = m.StreamNumber
                
            },
            m => new CourseSlotPresenterDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //CourseSlot = m.CourseSlot,
                //Presenter = m.Presenter
            })
        { }
    }
}
