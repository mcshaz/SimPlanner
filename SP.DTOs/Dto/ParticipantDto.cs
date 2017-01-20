using SP.DataAccess;
using SP.DataAccess.Data.Interfaces;
using SP.Dto.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SP.Dto
{
    [MetadataType(typeof(ParticipantDtoMetadata))]
    public class ParticipantDto //: IAdminApproved
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternateEmail { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string DietNotes { get; set; }
        public bool AdminApproved { get; set; }

        public Guid DefaultDepartmentId { get; set; }
        public Guid DefaultProfessionalRoleId { get; set; }
        public Guid? DrinkPreferenceId { get; set; }

        public virtual DepartmentDto Department { get; set; }
        public virtual ProfessionalRoleDto ProfessionalRole { get; set; }
        public virtual HotDrinkDto DrinkPreference { get; set; }

        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresentations { get; set; }
        public virtual ICollection<UserRoleDto> Roles { get; set; }
    }

    public static class ParticipantExtensions
    {
        public static void AddParticipants(this MailAddressCollection addresses, Participant participant)
        {
            AddParticipants(addresses, new[] { participant });
        }
        public static void AddParticipants(this MailAddressCollection addresses, IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
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
