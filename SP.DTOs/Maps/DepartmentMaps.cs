using SP.DataAccess;
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
                Colour = m.Colour
            },
            m => new DepartmentDto
            {
                Id = m.Id,
                Name = m.Name,
                InstitutionId = m.InstitutionId,
                InvitationLetterFilename = m.InvitationLetterFilename,
                CertificateFilename = m.CertificateFilename,
                Abbreviation = m.Abbreviation,
                Colour = m.Colour 
                //CourseTypes = null,
                //Institution = m.Institution,
                //Manequins = m.Manequins,
                //Courses = m.Courses,
                //Scenarios = m.Scenarios,
                //Departments = null
            })
        { }
    }
}
