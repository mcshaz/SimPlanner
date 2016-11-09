namespace SP.DataAccess
{
    using Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(InstitutionMetadata))]
    public class Institution : IAssociateFileOptional, IAdminApproved
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Abbreviation { get; set; }
        public string LocaleCode { get; set; }
        public string StandardTimeZone { get; set; }
        public string HomepageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool AdminApproved { get; set; }
        public string LogoImageFileName { get; set; }
        [NotMapped]
        public byte[] File { get; set; }
        [NotMapped]
        string IAssociateFile.FileName { get { return LogoImageFileName; } set { LogoImageFileName = value; } }

        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        public virtual Culture Culture { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<ProfessionalRoleInstitution> ProfessionalRoleInstitutions { get; set; }

        TimeZoneInfo _timeZone;
        [NotMapped]
        public TimeZoneInfo TimeZone
        {
            get { return _timeZone ?? (_timeZone = TimeZoneInfo.FindSystemTimeZoneById(StandardTimeZone)); }
        }

    }
}
