using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(CourseSlotManequinMetadata))]
    public class CourseSlotManequin
    {
        public Guid CourseId { get; set; }

        public Guid CourseSlotId { get; set; }

        public Guid ManequinId { get; set; }

        public byte StreamNumber { get; set; }

        public virtual Course Course { get; set; }

        public virtual CourseSlot CourseSlot { get; set; }

        public virtual Manequin Manequin { get; set; }
    }

    /*
    [MetadataType(typeof(ActivityFacultyRoleMetadata))]
    public class ActivityFacultyRole
    {
        public Guid CourseId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid FacultyMemberId { get; set; }

        public Guid RoleId { get; set; }

        public virtual Course Course { get; set; }
        public virtual CourseActivity Activity { get; set; }
        public virtual Activi Role { get; set; }
        public virtual Participant FacultyMember { get; set; }
    }
    */
}
