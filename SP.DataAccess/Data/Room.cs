using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.DataAccess
{
    [MetadataType(typeof(RoomMetadata))]
    public class Room : IAssociateFileOptional
    {
        public Guid Id { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Directions { get; set; }
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// institution map showing how to get to room
        /// </summary>
        public string FileName { get; set; }
        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        [NotMapped]
        public byte[] File { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection <Course> Courses { get; set; }
        public virtual ICollection<Course> FacultyMeetings { get; set; }
    }
}
