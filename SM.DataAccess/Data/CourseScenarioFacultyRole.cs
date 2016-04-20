using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.DataAccess
{
    [MetadataType(typeof(CourseScenarioFacultyRoleMetadata))]
    public class CourseScenarioFacultyRole
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid FacultyScenarioRoleId { get; set; }

        public Course Course { get; set; }
        public CourseSlot CourseSlot { get; set; }
        public Participant Participant { get; set; }
        public FacultyScenarioRole FacultyScenarioRole { get; set; }

    }
}
