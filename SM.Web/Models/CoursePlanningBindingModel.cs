using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Web.Models
{
    public class RsvpBindingModel
    {
        public Guid CourseId { get; set; }
        public Guid ParticipantId { get; set; }
        bool? _isAttending;
        public bool IsAttending { get { return _isAttending.Value; } }
        public int Attending { set { _isAttending = value != 0; }  }
    }

    public class EmailAllBindingModel
    {
        public Guid CourseId { get; set; }
    }
}
