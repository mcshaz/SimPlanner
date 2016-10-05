namespace SP.DataAccess
{
    using Data.Interfaces;
    using Helpers;
    using Metadata;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseParticipant : ITimeTracking
    {
        public Guid CourseId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ProfessionalRoleId { get; set; }

        public bool? IsConfirmed { get; set; }
        public bool IsFaculty { get; set; }
        public bool IsOrganiser { get; set; }

        private DateTime _createdUtc;
        public DateTime CreatedUtc
        {
            get
            {
                return _createdUtc;
            }
            set
            {
                _createdUtc = value.AsUtc();
            }
        }

        private DateTime _lastModifiedUtc;
        public DateTime LastModifiedUtc
        {
            get
            {
                return _lastModifiedUtc;
            }
            set
            {
                _lastModifiedUtc = value.AsUtc();
            }
        }

        public DateTime? EmailTimeStamp { get; set; }

        [NotMapped]
        public bool SystemChangesOnly { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Course Course { get; set; }
        /// <summary>
        /// During the course - this changes as candidate promoted
        /// </summary>
        public ProfessionalRole ProfessionalRole { get; set; }
        ///
        /// During the course - this changes as candidate completes different training allocations
        /// 
        public Department Department { get; set; }
    }
}
