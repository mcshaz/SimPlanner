using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Metadata
{
    public class RoomMetadata
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(32)]
        public string ShortDescription { get; set; }
        [StringLength(64)]
        public string FullDescription { get; set; }
        [StringLength(256)]
        public string Directions { get; set; }
    }
}
