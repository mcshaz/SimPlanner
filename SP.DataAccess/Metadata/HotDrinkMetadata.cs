using System.ComponentModel.DataAnnotations;

namespace SP.Metadata
{
    public class HotDrinkMetadata
    {
        [Required]
        [StringLength(64)]
        public string Description { get; set; }
    }
}
