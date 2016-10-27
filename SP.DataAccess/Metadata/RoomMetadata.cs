using SP.DataAccess.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Metadata
{
    public class RoomMetadata
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(32), Required]
        public string ShortDescription { get; set; }
        [StringLength(64)]
        public string FullDescription { get; set; }
        [StringLength(256)]
        public string Directions { get; set; }

        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        public long FileSize { get; set; }
    }
}
