using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(CourseSlotActivityMetadata))]
    public class CourseSlotActivity: IModified
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public byte StreamNumber { get; set; }
        public Guid? ScenarioId { get; set; }
        public Guid? ActivityId { get; set; }
        public DateTime Modified { get; set; }

        public virtual Course Course { get; set; }

        public virtual CourseSlot CourseSlot { get; set; }

        public virtual Scenario Scenario{ get; set; }

        public virtual Activity Activity { get; set; }
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
