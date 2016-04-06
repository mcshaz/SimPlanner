namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [MetadataType(typeof(InstitutionMetadata))]
    public class Institution
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public string LocaleCode { get; set; }

        public string StandardTimeZone { get; set; }

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
