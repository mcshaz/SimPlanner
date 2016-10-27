using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(RoomMetadata))]
    public class RoomDto: IAssociateFileOptional
    {
        public Guid Id { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Directions { get; set; }
        public Guid DepartmentId { get; set; }

        public string FileName { get; set; }
        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }

        public byte[] File { get; set; } 

        public DepartmentDto Department { get; set; }
        public virtual ICollection<CourseDto> Courses { get; set; }
        public virtual ICollection<CourseDto> FacultyMeetings { get; set; }
    }
}
