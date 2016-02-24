using SM.DataAccess;
using SM.Dto.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ParticipantDtoMetadata))]
    public class ParticipantDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternateEmail { get; set; }
        public string FullName { get; set; }
        public Guid DefaultDepartmentId { get; set; }
        public Guid DefaultProfessionalRoleId { get; set; }
        public DepartmentDto Department { get; set; }
        public ProfessionalRoleDto ProfessionalRole { get; set; }
        public ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

        internal static Func<ParticipantDto, Participant> mapToRepo = m => new Participant
        {
            Id = m.Id,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            AlternateEmail = m.AlternateEmail,
            FullName = m.FullName,
            DefaultDepartmentId = m.DefaultDepartmentId,
            DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,

            //Department = m.Department,

            //ProfessionalRole = m.ProfessionalRole,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };

        internal static Expression<Func<Participant, ParticipantDto>> mapFromRepo= m => new ParticipantDto
        {
            Id = m.Id,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            AlternateEmail = m.AlternateEmail,
            FullName = m.FullName,
            DefaultDepartmentId = m.DefaultDepartmentId,
            DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,

            //Department = m.Department,

            //ProfessionalRole = m.ProfessionalRole,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
    }
}
