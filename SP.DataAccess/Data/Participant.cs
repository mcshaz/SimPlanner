using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using SP.Metadata;
using SP.DataAccess.Data.Interfaces;

namespace SP.DataAccess
{
    [MetadataType(typeof(ParticipantMetadata))]
    public class Participant : IdentityUser<Guid,AspNetUserLogin,AspNetUserRole,AspNetUserClaim>, IAdminApproved
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
        public string DietNotes { get; set; }
        public bool AdminApproved { get; set; }
        public DateTime CreatedUtc { get; set; }

        public Guid DefaultDepartmentId { get; set; }
        public Guid DefaultProfessionalRoleId { get; set; }
        public Guid? DrinkPreferenceId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ProfessionalRole ProfessionalRole { get; set; }
        public virtual HotDrink DrinkPreference { get; set; }

        public virtual ICollection<CourseParticipant> CourseParticipants { get; set; }
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresentations { get; set; }
        public virtual ICollection<CourseFacultyInvite> CourseInvites { get; set; }
    }
}
