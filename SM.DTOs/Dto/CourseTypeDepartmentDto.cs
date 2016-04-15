using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
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
