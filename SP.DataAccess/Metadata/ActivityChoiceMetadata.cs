using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.DataAccess.Metadata
{
    public class ActivityChoiceMetadata
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [StringLength(64), Required]
        public string Description { get; set; }
        [StringLength(128)]
        public string FileResource { get; set; }
        public Guid CourseActivityId { get; set; }

        public virtual CourseActivity Activity { get; set; }
    }
}
