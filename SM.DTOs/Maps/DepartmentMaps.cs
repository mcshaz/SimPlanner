using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class DepartmentMaps
    {        internal static Func<DepartmentDto, Department> mapToRepo = m => new Department
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
