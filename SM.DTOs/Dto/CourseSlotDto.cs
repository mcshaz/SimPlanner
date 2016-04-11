using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlotDto 
    {
        public Guid Id { get; set; }
        public byte MinutesDuration { get; set; }
        public bool IsActive { get; set; }
        public byte Order { get; set; }
        public byte Day { get; set; }
        public byte SimultaneousStreams { get; set; }
        /// <summary>
        /// if activity/activityId is null, must be a scenario
        /// </summary>
        public Guid? ActivityId { get; set; }
        public Guid CourseFormatId { get; set; }
        public virtual CourseActivityDto Activity { get; set; }
        public virtual CourseFormatDto CourseFormat { get; set; }

        public virtual ICollection<ChosenTeachingResourceDto> ChosenTeachingResources { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotScenarioDto> CourseSlotScenarios { get; set; }
    }
}
