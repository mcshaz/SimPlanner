using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseSlotPresenterMaps
    {        internal static Func<CourseSlotPresenterDto, CourseSlotPresenter> MapToDomain()
        {
            return m => new CourseSlotPresenter
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                StreamNumber = m.StreamNumber
                
            };
        }


        internal static Expression<Func<CourseSlotPresenter, CourseSlotPresenterDto>> MapFromDomain()
        {
            return m => new CourseSlotPresenterDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //CourseSlot = m.CourseSlot,
                //Presenter = m.Presenter
            };
        }
    }
}
