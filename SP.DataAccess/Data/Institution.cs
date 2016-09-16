namespace SP.DataAccess
{
    using SP.Metadata;
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

        public string Abbreviation { get; set; }

        public string LocaleCode { get; set; }

        public string StandardTimeZone { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool AdminApproved { get; set; }

        public virtual Culture Culture { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfessionalRoleInstitution> ProfessionalRoleInstitutions { get; set; }

    }
}
