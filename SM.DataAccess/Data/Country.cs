namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Country")]
    public partial class Country
    {

        [Key]
        [StringLength(2)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        
        private ICollection<Hospital> _hospitals;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hospital> Hospitals
        {
            get { return _hospitals ?? (_hospitals = new List<Hospital>()); }
            set { _hospitals = value; }
        }

        
        private ICollection<ProfessionalRole> _professionalRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfessionalRole> ProfessionalRoles
        {
            get { return _professionalRoles ?? (_professionalRoles = new List<ProfessionalRole>()); }
            set { _professionalRoles = value; }
        }
    }
}
