using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.DataAccess
{
    //todo map 
    //ApplicationUser : IdentityUser<int>
    // Add profile data for application users by adding properties to the ApplicationUser class
    [Table("AspNetUsers")]
    public class Participant 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required, EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string SecondaryEmail { get; set; }


        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public int DefaultDepartmentId { get; set; }

        public int DefaultProfessionalRoleId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ProfessionalRole ProfessionalRole { get; set; }

        private ICollection<InstructorCourseParticipant> _instructorCourseParticipant;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstructorCourseParticipant> InstructorCourses
        {
            get { return _instructorCourseParticipant ?? (_instructorCourseParticipant = new List<InstructorCourseParticipant>()); }
            set { _instructorCourseParticipant = value; }
        }
        private ICollection<SessionParticipant> _sessionParticipants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionParticipant> SessionParticipants
        {
            get { return _sessionParticipants ?? (_sessionParticipants = new List<SessionParticipant>()); }
            set { _sessionParticipants = value; }
        }
    }
}
