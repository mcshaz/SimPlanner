namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Department")]
    public partial class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int HospitalId { get; set; }

        [StringLength(256)]
        public string InvitationLetterFilename { get; set; }

        [StringLength(256)]
        public string CertificateFilename { get; set; }

        private ICollection<Participant> _participants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant> Participants
        {
            get { return _participants ?? (_participants = new List<Participant>()); }
            set { _participants = value; }
        }

        private ICollection<Hospital> _hospitals;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hospital> Hospitals
        {
            get { return _hospitals ?? (_hospitals = new List<Hospital>()); }
            set { _hospitals = value; }
        }

        private ICollection<Manequin> _manequins;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manequin> Manequins
        {
            get { return _manequins ?? (_manequins = new List<Manequin>()); }
            set { _manequins = value; }
        }

        private ICollection<Session> _sessions;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions
        {
            get { return _sessions ?? (_sessions = new List<Session>()); }
            set { _sessions = value; }
        }
    }
}

