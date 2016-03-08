using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(RoomMetadata))]
    public class RoomDto
    {
        public Guid Id { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Directions { get; set; }
        public Guid DepartmentId { get; set; }
        public DepartmentDto Department { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
    }
}
