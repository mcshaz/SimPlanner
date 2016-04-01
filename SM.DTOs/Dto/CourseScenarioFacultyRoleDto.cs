using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseScenarioFacultyRoleMetadata))]
    public class CourseScenarioFacultyRoleDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid FacultyMemberId { get; set; }
        public Guid FacultySimRoleId { get; set; }

        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ParticipantDto FacultyMember { get; set; }
        public FacultySimRoleDto FacultySimRole { get; set; }

    }
}
