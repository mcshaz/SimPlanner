using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ActivityChoiceMetadata))]
    public class ActivityChoice
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FileResource { get; set; }
        public Guid CourseActivityId { get; set; }

        public virtual CourseActivity Activity { get; set; }
    }
}
