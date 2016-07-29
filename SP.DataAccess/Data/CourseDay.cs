namespace SP.DataAccess
{
    using Data.Interfaces;
    using Helpers;
    using SP.Metadata;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseDayMetadata))]
    public class CourseDay : ICourseDay
    {
        public Guid CourseId { get; set; }
        public int Day { get; set; }

        private DateTime _startUtc;
        public DateTime StartUtc
        {
            get
            {
                return _startUtc;
            }
            set
            {
                _startUtc = value.AsUtc();
            }
        }

        public TimeSpan Duration { get; set; }

        public virtual Course Course {get; set;}

    }
}
