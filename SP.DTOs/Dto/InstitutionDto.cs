using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Dto
{
    [MetadataType(typeof(InstitutionMetadata))]
    public class InstitutionDto : IAssociateFileOptional, IAdminApproved
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Abbreviation { get; set; }
        public string LocaleCode { get; set; }
        public string StandardTimeZone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string LogoImageFileName { get; set; }
        public bool AdminApproved { get; set; }
        [NotMapped]
        string IAssociateFile.FileName { get { return LogoImageFileName; } set { LogoImageFileName = value; } }

        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        public byte[] File { get; set; }

        public CultureDto Culture { get; set; }
        public virtual ICollection<DepartmentDto> Departments { get; set; }
        public virtual ICollection<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get; set; }
        public virtual ICollection<ResourceSharingInstitutionDto> ResourceGivingInstitutions { get; set; }
        public virtual ICollection<ResourceSharingInstitutionDto> ResourceReceivingInstitutions { get; set; }
    }
}
