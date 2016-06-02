using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(CourseTypeDepartmentMetadata))]
    public class CourseTypeDepartmentDto
    {
        public Guid CourseTypeId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual CourseTypeDto CourseType { get; set; }
        public virtual DepartmentDto Department { get; set; }
    }
}
