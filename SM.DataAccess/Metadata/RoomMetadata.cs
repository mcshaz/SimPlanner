using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{
    public class RoomMetadata
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(64)]
        public string Description { get; set; }
        [StringLength(256)]
        public string Directions { get; set; }
    }
}
