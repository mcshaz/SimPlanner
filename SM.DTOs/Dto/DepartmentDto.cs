using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(DepartmentMetadata))]
    public class DepartmentDto
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid InstitutionId { get; set; }
        public string InvitationLetterFilename { get; set; }
        public string CertificateFilename { get; set; }
        public ICollection<CourseTypeDto> CourseTypes { get; set; }
        public InstitutionDto Institution { get; set; }
        public ICollection<ManequinDto> Manequins { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
        public ICollection<ScenarioDto> Scenarios { get; set; }
        public ICollection<ParticipantDto> Participants { get; set; }


        internal static Func<DepartmentDto, Department> mapToRepo = m => new Department
        {
            Id = m.Id,
            Name = m.Name,
            InstitutionId = m.InstitutionId,
            InvitationLetterFilename = m.InvitationLetterFilename,
            CertificateFilename = m.CertificateFilename
        };


        internal static Expression<Func<Department, DepartmentDto>> mapFromRepo = m => new DepartmentDto
        {
            Id = m.Id,
            Name = m.Name,
            InstitutionId = m.InstitutionId,
            InvitationLetterFilename = m.InvitationLetterFilename,
            CertificateFilename = m.CertificateFilename

            //CourseTypes = m.CourseTypes,
            //Institution = m.Institution,
            //Manequins = m.Manequins,
            //Courses = m.Courses,
            //Scenarios = m.Scenarios,
            //Departments = m.Departments
        };
    }
}
