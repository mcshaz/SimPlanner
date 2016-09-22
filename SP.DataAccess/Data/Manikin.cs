namespace SP.DataAccess
{
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ManikinMetadata))]
    public partial class Manikin
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid ModelId { get; set; }

        public bool PurchasedNew { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? LocalCurrencyPurchasePrice { get; set; }

        public DateTime? DecommissionDate { get; set; }

        public string DecommissionReason { get; set; }

        public virtual Department Department { get; set; }

        public virtual ManikinModel Model { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManikinService> ManikinServices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotManikin> CourseSlotManikins { get; set; }
    }
}
