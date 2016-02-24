namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class Institution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        //[AllowHtml]
        public string About { get; set; }

        [StringLength(2)]
        public string CountryCode { get; set; }

        public virtual Country Country { get; set; }

		ICollection<Department> _departments; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments
		{
			get
			{
				return _departments ?? (_departments = new List<Department>());
			}
			set
			{
				_departments = value;
			}
		}
    }
}
