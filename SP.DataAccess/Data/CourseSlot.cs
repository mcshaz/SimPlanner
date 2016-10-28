using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlot : IModified
    {
        public Guid Id { get; set; }
        public byte MinutesDuration { get; set; }
        public int Order { get; set; }
        public byte Day { get; set; }
        public bool IsActive { get; set; }
        public bool TrackParticipants { get; set; }
        public byte SimultaneousStreams { get; set; }
        public Guid? ActivityId { get; set; }
        public Guid CourseFormatId { get; set; }
        public DateTime Modified { get; set; }

        public virtual CourseActivity Activity { get; set; }
        public virtual CourseFormat CourseFormat { get; set; }

        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters { get; set; }
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotActivity> CourseSlotActivities { get; set; }
        public virtual ICollection<CourseSlotManikin> CourseSlotManikins { get; set; }

    }
}
