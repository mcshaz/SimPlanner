using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CourseSlotPresenterMaps
    {        internal static Func<CourseSlotPresenterDto, CourseSlotPresenter> mapToRepo = m => new CourseSlotPresenter
        {
            CourseId = m.CourseId,
            CourseSlotId = m.CourseSlotId,
            PresenterId = m.PresenterId
        };


        internal static Expression<Func<CourseSlotPresenter, CourseSlotPresenterDto>> mapFromRepo = m => new CourseSlotPresenterDto
        {
            CourseId = m.CourseId,
            CourseSlotId = m.CourseSlotId,
            PresenterId = m.PresenterId,
            //Course = m.Course,
            //CourseSlot = m.CourseSlot,
            //Presenter = m.Presenter
        };
    }
}
