namespace SP.DataAccess
{
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ManequinMetadata))]
    public partial class Manequin
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid? DepartmentId { get; set; }

        public Guid ModelId { get; set; }

        public bool PurchasedNew { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? LocalCurrencyPurchasePrice { get; set; }

        public DateTime? DecommissionDate { get; set; }

        public string DecommissionReason { get; set; }

        public virtual Department Department { get; set; }

        public virtual ManequinModel Model { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManequinService> ManequinServices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotManequin> CourseSlotManequins { get; set; }
    }
}
