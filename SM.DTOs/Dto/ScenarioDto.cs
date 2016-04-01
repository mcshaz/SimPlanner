using SM.DataAccess;
using SM.DataAccess.Enums;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(ScenarioMetadata))]
    public class ScenarioDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public Difficulty Complexity { get; set; }
        public Emersion? EmersionCategory { get; set; }
        public string TemplateFilename { get; set; }
        public Guid? ManequinId { get; set; }
        public Guid CourseTypeId { get; set; }

        public virtual ManequinDto Manequin { get; set; }
        public virtual CourseTypeDto CourseType { get; set; }
        public virtual DepartmentDto Department { get; set; }

        public virtual ICollection<CourseDto> Courses { get; set; }
        public virtual ICollection<ScenarioResourceDto> ScenarioResources { get; set; }
        public virtual ICollection<CourseSlotScenarioDto> CourseSlotScenarios { get; set; }
    }
}
