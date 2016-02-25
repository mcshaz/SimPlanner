namespace SM.DataAccess
{
    using Metadata;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseParticipant
    {
        public Guid ParticipantId { get; set; }

        public Guid CourseId { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsFaculty { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid ProfessionalRoleId { get; set; }

        public virtual Participant Participant { get; set; }

        public virtual Course Course { get; set; }
    }
}
