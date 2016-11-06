using SP.DataAccess;
using SP.Dto.ProcessBreezeRequests;

namespace SP.Dto.Maps
{
    internal class DepartmentMaps : DomainDtoMap<Department, DepartmentDto>
    {
        public DepartmentMaps() : base(m => new Department
            {
                Id = m.Id,
                Name = m.Name,
                InstitutionId = m.InstitutionId,
                InvitationLetterFilename = m.InvitationLetterFilename,
                CertificateFilename = m.CertificateFilename,
                Abbreviation = m.Abbreviation,
                PrimaryColour = m.PrimaryColour,
                SecondaryColour = m.SecondaryColour,
                AdminApproved = m.AdminApproved
            },
            m => new DepartmentDto
            {
                Id = m.Id,
                Name = m.Name,
                InstitutionId = m.InstitutionId,
                InvitationLetterFilename = m.InvitationLetterFilename,
                CertificateFilename = m.CertificateFilename,
                Abbreviation = m.Abbreviation,
                PrimaryColour = m.PrimaryColour,
                SecondaryColour = m.SecondaryColour,
                AdminApproved = m.AdminApproved
            })
        {
            WherePredicate = v =>
            {
                if (v.AdminLevel == AdminLevels.AllData)
                {
                    return null;
                }
                return d => d.AdminApproved;
            };
        }
    }
}
