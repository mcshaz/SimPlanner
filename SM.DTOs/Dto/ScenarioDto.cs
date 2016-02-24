using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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
        public ManequinDto Manequin { get; set; }
        public CourseTypeDto CourseType { get; set; }
        public DepartmentDto Department { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
        public ICollection<ScenarioResourceDto> Resources { get; set; }
        public ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

        internal static Func<ScenarioDto, Scenario>  mapToRepo = m => new Scenario {
            Id = m.Id,
            Description = m.Description,
            DepartmentId = m.DepartmentId,
            Complexity = m.Complexity,
            EmersionCategory = m.EmersionCategory,
            TemplateFilename = m.TemplateFilename,
            ManequinId = m.ManequinId,
            CourseTypeId = m.CourseTypeId,
            //Manequin = m.Manequin,
            //CourseType = m.CourseType,
            //Department = m.Department,
            //Courses = m.Courses,
            //Resources = m.Resources,
            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };

        internal static Expression<Func<Scenario, ScenarioDto>> mapFromRepo= m => new ScenarioDto
        {
            Id = m.Id,
            Description = m.Description,
            DepartmentId = m.DepartmentId,
            Complexity = m.Complexity,
            EmersionCategory = m.EmersionCategory,
            TemplateFilename = m.TemplateFilename,
            ManequinId = m.ManequinId,
            CourseTypeId = m.CourseTypeId,
            //Manequin = m.Manequin,
            //CourseType = m.CourseType,
            //Department = m.Department,
            //Courses = m.Courses,
            //Resources = m.Resources,
            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
    }
}
