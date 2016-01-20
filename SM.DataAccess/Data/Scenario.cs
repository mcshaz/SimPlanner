namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Scenario")]
    public partial class Scenario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Description { get; set; }

        public int DepartmentId { get; set; }

        public Difficulty Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        [Required]
        [StringLength(256)]
        public string TemplateFilename { get; set; }

        public int? ManequinId { get; set; }

        public int SessionTypeId { get; set; }

        public virtual Manequin Manequin { get; set; }

        public virtual SessionType SessionType { get; set; }

        private ICollection<Session> _sessions;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions
        {
            get { return _sessions ?? (_sessions = new List<Session>()); }
            set { _sessions = value; }
        }

        private ICollection<ScenarioResource> _resources;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> Resources
        {
            get { return _resources ?? (_resources = new List<ScenarioResource>()); }
            set { _resources = value; }
        }

        private ICollection<ScenarioRoleType> _roles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioRoleType> Roles
        {
            get { return _roles ?? (_roles = new List<ScenarioRoleType>()); }
            set { _roles = value; }
        }
    }
}
