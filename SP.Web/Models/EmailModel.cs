using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SP.Web.Models
{
    public class EmailModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}