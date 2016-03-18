namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(ManequinMetadata))]
    public partial class Manequin
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid? DepartmentId { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ManequinManufacturer Manufacturer { get; set; }

        ICollection<Scenario> _scenarios; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
		{
			get
			{
				return _scenarios ?? (_scenarios = new List<Scenario>());
			}
			set
			{
				_scenarios = value;
			}
		}
    }
}
