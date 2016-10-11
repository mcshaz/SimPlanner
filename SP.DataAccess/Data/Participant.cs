using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using SP.Metadata;
using System.Net.Mail;

namespace SP.DataAccess
{
    [MetadataType(typeof(ParticipantMetadata))]
    public class Participant : IdentityUser<Guid,AspNetUserLogin,AspNetUserRole,AspNetUserClaim>
    {
        #region overrides 
        //specifying override allows required attribute from metadata
        public override string Email
        {
            get
            {
                return base.Email;
            }
            set
            {
                base.Email = value;
            }
        }
        #endregion //overrides

        public string AlternateEmail { get; set; }

        public string FullName { get; set; }

        public Guid DefaultDepartmentId { get; set; }

        public Guid DefaultProfessionalRoleId { get; set; }

        public Guid? DrinkPreferenceId { get; set; }

        public string DietNotes { get; set; }

        public virtual Department Department { get; set; }

        public virtual ProfessionalRole ProfessionalRole { get; set; }

        public virtual HotDrink DrinkPreference { get; set; }

		ICollection<CourseParticipant> _courseParticipants; 

        public virtual ICollection<CourseParticipant> CourseParticipants
		{
			get
			{
				return _courseParticipants ?? (_courseParticipants = new List<CourseParticipant>());
			}
			set
			{
				_courseParticipants = value;
			}
		}

        ICollection<CourseScenarioFacultyRole> _courseScenarioFacultyRoles;

        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles
        {
            get
            {
                return _courseScenarioFacultyRoles ?? (_courseScenarioFacultyRoles = new List<CourseScenarioFacultyRole>());
            }
            set
            {
                _courseScenarioFacultyRoles = value;
            }
        }

        ICollection<CourseSlotPresenter> _courseSlotPresentations;

        public virtual ICollection<CourseSlotPresenter> CourseSlotPresentations
        {
            get
            {
                return _courseSlotPresentations ?? (_courseSlotPresentations = new List<CourseSlotPresenter>());
            }
            set
            {
                _courseSlotPresentations = value;
            }
        }
    }

    public static class ParticipantExtensions
    {
        public static void AddParticipants(this MailAddressCollection addresses, Participant participant)
        {
            AddParticipants(addresses, new[] { participant });
        }
        public static void AddParticipants(this MailAddressCollection addresses, IEnumerable<Participant> participants)
        {
            foreach(var participant in participants)
            {
                addresses.Add(new MailAddress(participant.Email, participant.FullName));
                if (participant.AlternateEmail != null)
                {
                    addresses.Add(new MailAddress(participant.AlternateEmail, participant.FullName));
                }
            }
        }
    }
}
