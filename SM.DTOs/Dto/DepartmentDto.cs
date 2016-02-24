using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


    }
}
