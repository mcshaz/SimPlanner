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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Hospitals { get; set; }

		ICollection<ProfessionalRole> _professionalRoles; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfessionalRole> ProfessionalRoles
		{
			get
			{
				return _professionalRoles ?? (_professionalRoles = new List<ProfessionalRole>());
			}
			set
			{
				_professionalRoles = value;
			}
		}
    }
}
