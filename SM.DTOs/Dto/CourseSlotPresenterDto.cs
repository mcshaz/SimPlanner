using SM.DataAccess;
using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotPresenterMetadata))]
    public class CourseSlotPresenterDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid PresenterId { get; set; }
        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ParticipantDto Presenter { get; set; }

        internal static Func<CourseSlotPresenterDto, CourseSlotPresenter> mapToRepo = m => new CourseSlotPresenter
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
