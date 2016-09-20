using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(ManikinMetadata))]
    public class ManikinDto
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

        public virtual DepartmentDto Department { get; set; }

        public virtual ManikinModelDto Model { get; set; }

        public virtual ICollection<ManikinServiceDto> ManikinServices { get; set; }

        public virtual ICollection<CourseSlotManikinDto> CourseSlotManikins { get; set; }
    }
}
