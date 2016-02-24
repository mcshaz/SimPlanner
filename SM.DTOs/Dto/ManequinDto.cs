using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    }
}
