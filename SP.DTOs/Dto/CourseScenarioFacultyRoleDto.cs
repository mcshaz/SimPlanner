using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(CourseScenarioFacultyRoleMetadata))]
    public class CourseScenarioFacultyRoleDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid FacultyScenarioRoleId { get; set; }

        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ParticipantDto Participant { get; set; }
        public FacultyScenarioRoleDto FacultyScenarioRole { get; set; }

    }
}
