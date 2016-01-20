namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SessionRoleType
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        private ICollection<SessionType> _sessionTypes;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionType> SessionTypes
        {
            get { return _sessionTypes ?? (_sessionTypes = new List<SessionType>());  }
            set { _sessionTypes = value; }
        }
    }
}
