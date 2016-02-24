using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using SM.Metadata;

namespace SM.DataAccess
{
    [MetadataType(typeof(ParticipantMetadata))]
    public class Participant : IdentityUser<Guid,AspNetUserLogin,AspNetUserRole,AspNetUserClaim>
    {
        #region overrides //note overiding a few properties so that referencing assemblies do not need to reference aspnet.identity.entityframework
        public override Guid Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }

        public override string UserName
        {
            get
            {
                return base.UserName;
            }

            set
            {
                base.UserName = value;
            }
        }

        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }

            set
            {
                base.PhoneNumber = value;
            }
        }

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
        #endregion //overrides


        public string AlternateEmail { get; set; }

        public string FullName { get; set; }

        public Guid DefaultDepartmentId { get; set; }

        public Guid DefaultProfessionalRoleId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ProfessionalRole ProfessionalRole { get; set; }

		ICollection<CourseParticipant> _sessionParticipants; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> CourseParticipants
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

        ICollection<ScenarioFacultyRole> _scenarioFacultyRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioFacultyRole> ScenarioFacultyRoles
        {
            get
            {
                return _scenarioFacultyRoles ?? (_scenarioFacultyRoles = new List<ScenarioFacultyRole>());
            }
            set
            {
                _scenarioFacultyRoles = value;
            }
        }
    }
}
