﻿using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotManequinMetadata))]
    public class CourseSlotManequinDto
    {
        public Guid CourseId { get; set; }

        public Guid CourseSlotId { get; set; }

        public Guid ManequinId { get; set; }

        public byte StreamNumber { get; set; }

        public virtual CourseDto Course { get; set; }

        public virtual CourseSlotDto CourseSlot { get; set; }

        public virtual ManequinDto Manequin { get; set; }
    }

    /*
    [MetadataType(typeof(ActivityFacultyRoleMetadata))]
    public class ActivityFacultyRole
    {
        public Guid CourseId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid FacultyMemberId { get; set; }

        public Guid RoleId { get; set; }

        public virtual Course Course { get; set; }
        public virtual CourseActivity Activity { get; set; }
        public virtual Activi Role { get; set; }
        public virtual Participant FacultyMember { get; set; }
    }
    */
}
