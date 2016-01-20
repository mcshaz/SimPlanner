namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProfessionalRole")]
    public partial class ProfessionalRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public ProfessionalCategory Category { get; set; }

        private ICollection<Participant> _participants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant> Participants
        {
            get { return _participants ?? (_participants = new List<Participant>());}
            set { _participants = value; }
        }

        private ICollection<Country> _countries ;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Country> Countries
        {
            get { return _countries ?? (_countries = new List<Country>()); }
            set { _countries = value; }
        }
    }
}
