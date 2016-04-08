using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseSlotPresenterMaps
    {        internal static Func<CourseSlotPresenterDto, CourseSlotPresenter> mapToRepo()
        {
            return m => new CourseSlotPresenter
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId
            };
        }


        internal static Expression<Func<CourseSlotPresenter, CourseSlotPresenterDto>> mapFromRepo()
        {
            return m => new CourseSlotPresenterDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                //Course = m.Course,
                //CourseSlot = m.CourseSlot,
                //Presenter = m.Presenter
            };
        }
    }
}
