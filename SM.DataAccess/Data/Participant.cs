using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SM.DataAccess
{
    public class Participant : IdentityUser<Guid,AspNetUserLogin,AspNetUserRole,AspNetUserClaim>
    {

        public override string Email
        {
            get
            {
                return base.Email;
            }

            set
            {
                base.Email = value;
                if (string.IsNullOrEmpty(base.UserName))
                {
                    base.UserName = value;
                }
            }
        }
        [EmailAddress]
        [StringLength(256)]
        public string AlternateEmail { get; set; }

        public string FullName { get; set; }

        public int? DefaultDepartmentId { get; set; }

        public int? DefaultProfessionalRoleId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ProfessionalRole ProfessionalRole { get; set; }

		ICollection<CourseParticipant> _sessionParticipants; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> SessionParticipants
		{
			get
			{
				return _sessionParticipants ?? (_sessionParticipants = new List<CourseParticipant>());
			}
			set
			{
				_sessionParticipants = value;
			}
		}
    }
}
