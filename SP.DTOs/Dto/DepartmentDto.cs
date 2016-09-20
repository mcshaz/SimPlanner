using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(DepartmentMetadata))]
    public class DepartmentDto
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Guid InstitutionId { get; set; }
        public string InvitationLetterFilename { get; set; }
        public string CertificateFilename { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
        public InstitutionDto Institution { get; set; }
        public virtual ICollection<CourseTypeDepartmentDto> CourseTypeDepartments { get; set; }
        public virtual ICollection<ManikinDto> Manikins { get; set; }
        public virtual ICollection<CourseDto> Courses { get; set; }
        public virtual ICollection<CourseDto> OutreachCourses { get; set; }
        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
        public virtual ICollection<ParticipantDto> Participants { get; set; }
        public virtual ICollection<RoomDto> Rooms { get; set; }
        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
    }
}
