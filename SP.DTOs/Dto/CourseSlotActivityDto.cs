using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(CourseSlotActivityMetadata))]
    public class CourseSlotActivityDto
    {
        public Guid CourseId { get; set; }

        public Guid CourseSlotId { get; set; }

        public Guid? ScenarioId { get; set; }

        public Guid? ActivityId { get; set; }

        public byte StreamNumber { get; set; }

        public virtual CourseDto Course { get; set; }

        public virtual CourseSlotDto CourseSlot { get; set; }

        public virtual ScenarioDto Scenario{ get; set; }

        public virtual ActivityDto Activity { get; set; }
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
