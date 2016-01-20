namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SessionType")]
    public partial class SessionType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public Emersion? EmersionCategory { get; set; }

        [Required]
        [StringLength(10)]
        public string Abbrev { get; set; }

        private ICollection<Scenario> _scenarios;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
        {
            get { return _scenarios ?? (_scenarios = new List<Scenario>()); }
            set { _scenarios = value; }
        }

        private ICollection<Session> _sessions;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions
        {
            get { return _sessions ?? (_sessions = new List<Session>()); }
            set { _sessions = value; }
        }

        private ICollection<SessionRoleType> _rolesRequired;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionRoleType> RolesRequired
        {
            get { return _rolesRequired ?? (_rolesRequired = new List<SessionRoleType>()); }
            set { _rolesRequired = value; }
        }
    }
}
