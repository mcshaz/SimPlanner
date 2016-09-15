using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Web.Models
{
    public class DowloadFileSetModel
    {
        [Required]
        public Guid EntitySetId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Token { get; set; }
    }
}