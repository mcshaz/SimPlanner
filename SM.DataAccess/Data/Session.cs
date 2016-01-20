namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Session")]
    public partial class Session
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int DepartmentId { get; set; }

        public int? OutreachingDepartmentId { get; set; }

        public byte FacultyRequired { get; set; }

        [StringLength(256)]
        public string ParticipantVideoFilename { get; set; }

        [StringLength(256)]
        public string FeedbackSummaryFilename { get; set; }

        public int SessionTypeId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Department OutreachingDepartment { get; set; }

        public virtual SessionType SessionType { get; set; }

        private ICollection<SessionParticipant> _sessionParticipants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionParticipant> SessionParticipants
        {
            get { return _sessionParticipants ?? (_sessionParticipants = new List<SessionParticipant>()); }
            set { _sessionParticipants = value; }
        }

        private ICollection<Scenario> _scenario;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
        {
            get { return _scenario ?? (_scenario = new List<Scenario>()); }
            set { _scenario = value; }
        }

        private ICollection<SessionRoleType> _roles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionRoleType> Roles
        {
            get { return _roles ?? (_roles = new List<SessionRoleType>()); }
            set { _roles = value; }
        }

        private ICollection<SessionResource> _resources;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionResource> Resources
        {
            get { return _resources ?? (_resources = new List <SessionResource> ()); }
            set { _resources = value; }
        }
    }
}
