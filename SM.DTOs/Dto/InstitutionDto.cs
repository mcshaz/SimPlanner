using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(InstitutionMetadata))]
    public class InstitutionDto
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string LocaleCode { get; set; }
        public string StandardTimeZone { get; set; }
        public CountryDto Country { get; set; }
        public virtual ICollection<DepartmentDto> Departments { get; set; }
        public virtual ICollection<ProfessionalRoleDto> ProfessionalRoles { get; set; }

    }
}
