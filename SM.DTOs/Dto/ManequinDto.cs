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
        public Guid? DepartmentId { get; set; }
        public Guid ManufacturerId { get; set; }
        public DepartmentDto Department { get; set; }
        public ManequinManufacturerDto Manufacturer { get; set; }
        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
    }
}
