namespace SP.Dto
{
    using DataAccess.Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseDayMetadata))]
    public class CourseDayDto : ICourseDay
    {
        public Guid CourseId { get; set; }
        public int Day { get; set; }
        public DateTime StartFacultyUtc { get; set; }
        public int DurationFacultyMins { get; set; }
        public DateTime StartParticipantUtc { get; set; }
        public int DurationParticipantMins { get; set; }

        public virtual CourseDto Course {get; set;}

    }
}
