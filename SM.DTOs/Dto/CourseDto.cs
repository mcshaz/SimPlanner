using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CourseMetadata))]
    public class CourseDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FacultyMeetingTime { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public Guid RoomId { get; set; }
        public byte FacultyNoRequired { get; set; }
        public Guid CourseTypeId { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }

        public DepartmentDto Department { get; set; }
        public DepartmentDto OutreachingDepartment { get; set; }
        public CourseTypeDto CourseType { get; set; }
        public RoomDto Room { get; set; }

        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
        public virtual ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
    }

}
