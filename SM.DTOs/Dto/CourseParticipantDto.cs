using SM.DataAccess;
using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseParticipantDto
	{
        public Guid ParticipantId { get; set; }
        public Guid CourseId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsFaculty { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ProfessionalRoleId { get; set; }
        public ParticipantDto Participant { get; set; }
        public CourseDto Course { get; set; }

        internal static Func<CourseParticipantDto, CourseParticipant> mapToRepo = m => new CourseParticipant
        {
            ParticipantId = m.ParticipantId,
            CourseId = m.CourseId,
            IsConfirmed = m.IsConfirmed,
            IsFaculty = m.IsFaculty,
            DepartmentId = m.DepartmentId,
            ProfessionalRoleId = m.ProfessionalRoleId
            //Participant = m.Participant,
            //Course = m.Course
        };

        internal static Expression<Func<CourseParticipant, CourseParticipantDto>> mapFromRepo= m => new CourseParticipantDto
        {
            ParticipantId = m.ParticipantId,
            CourseId = m.CourseId,
            IsConfirmed = m.IsConfirmed,
            IsFaculty = m.IsFaculty,
            DepartmentId = m.DepartmentId,
            ProfessionalRoleId = m.ProfessionalRoleId
            //Participant = m.Participant,
            //Course = m.Course
        };
    }
}
