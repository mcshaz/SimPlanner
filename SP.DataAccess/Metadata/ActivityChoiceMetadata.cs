using SP.DataAccess;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
    public class ActivityChoiceMetadata : ResourceMetadata
    {
        public Guid CourseActivityId { get; set; }

        public virtual CourseActivity Activity { get; set; }
    }
}
