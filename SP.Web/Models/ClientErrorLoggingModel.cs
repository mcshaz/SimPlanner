using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SP.Web.Models
{
    public class ClientErrorLoggingModel
    {
        public string Source { get; set; }
        public string JsonData { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; }
    }
}