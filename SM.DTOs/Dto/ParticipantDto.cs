using SM.Dto.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresentations { get; set; }
    }
}
