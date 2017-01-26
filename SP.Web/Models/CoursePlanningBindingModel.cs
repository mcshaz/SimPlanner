using System;
using System.Collections.Generic;

namespace SP.Web.Models
{
    public class RsvpBindingModel
    {
        public Guid CourseId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid? Auth { get; set; }
        bool? _isAttending;
        public bool IsAttending { get { return _isAttending.Value; } }
        public int Attending { set { _isAttending = value != 0; }  }
    }

    public class EmailAllBindingModel
    {
        public Guid CourseId { get; set; }
    }

    public class MultiInviteBindingModel
    {
        public ICollection<Guid> Invitees { get; set; }
        public ICollection<Guid> Courses { get; set; }
    }
}
