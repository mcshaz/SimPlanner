using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(RoomMetadata))]
    public class Room
    {
        public Guid Id { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Directions { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
        public virtual ICollection <Course> Courses { get; set; }
        public virtual ICollection<Course> FacultyMeetings { get; set; }
    }
}
