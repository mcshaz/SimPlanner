using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ManequinMetadata))]
    public class ManequinDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }
        public int ManufacturerId { get; set; }
        public DepartmentDto Department { get; set; }
        public ICollection<ScenarioDto> Scenarios { get; set; }
        public ManequinManufacturerDto Manufacturer { get; set; }

        internal static Func<ManequinDto, Manequin> mapToRepo = m => new Manequin
        {
            Id = m.Id,
            Description = m.Description,
            DepartmentId = m.DepartmentId,
            ManufacturerId = m.ManufacturerId,
        };

        internal static Expression<Func<Manequin, ManequinDto>> mapFromRepo= m => new ManequinDto
        {
            Id = m.Id,
            Description = m.Description,
            DepartmentId = m.DepartmentId,
            ManufacturerId = m.ManufacturerId,
            //Department = m.Department,
            //Scenarios = m.Scenarios,
            //Manufacturer = m.Manufacturer
        };
    }
}
