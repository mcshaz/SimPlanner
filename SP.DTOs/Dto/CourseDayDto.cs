namespace SP.Dto
{
    using SP.Metadata;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseDayMetadata))]
    public class CourseDayDto
    {
        public Guid CourseId { get; set; }

        public int Day { get; set; }

        public DateTime StartUtc { get; set; }

        public int DurationMins { get; set; }

        public virtual CourseDto Course {get; set;}

    }
}
