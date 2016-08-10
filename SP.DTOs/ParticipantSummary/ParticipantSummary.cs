using System.Collections.Generic;

namespace SP.DTOs.ParticipantSummary
{
    public class ParticipantSummary
    {
        public IEnumerable<ParticipantCourseSummary> CourseSummary { get; set; }
        public IEnumerable<FacultySimRoleSummary> SimRoleSummary { get; set; }
        public IEnumerable<FacultyPresentationRoleSummary> PresentationSummary { get; set; }
    }
}
