using SP.DataAccess;
using System;
using System.Data.SqlTypes;
using System.Linq;

namespace SP.Dto.Maps
{
    public class CourseParticipantMaps: DomainDtoMap<CourseParticipant, CourseParticipantDto>
    {
        public CourseParticipantMaps(): base(m => new CourseParticipant
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId,
                IsConfirmed = m.IsConfirmed,
                IsFaculty = m.IsFaculty,
                IsOrganiser = m.IsOrganiser,
                DepartmentId = m.DepartmentId,
                ProfessionalRoleId = m.ProfessionalRoleId
            }, 
            m => new CourseParticipantDto
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId,
                IsConfirmed = m.IsConfirmed,
                IsFaculty = m.IsFaculty,
                IsOrganiser = m.IsOrganiser,
                DepartmentId = m.DepartmentId,
                ProfessionalRoleId = m.ProfessionalRoleId,
                IsEmailed = m.EmailedUtc.HasValue && (m.EmailedUtc.Value > m.Course.CourseDatesLastModified
                                    || (m.IsFaculty && ((m.Course.CourseFormat.CourseSlots.Any() && m.EmailedUtc > m.Course.CourseFormat.CourseSlots.Max(cs => cs.Modified))
                                        || (m.Course.CourseSlotPresenters.Any() && m.EmailedUtc > m.Course.CourseSlotPresenters.Max(cs => cs.Modified))
                                        || (m.Course.CourseSlotManikins.Any() && m.EmailedUtc > m.Course.CourseSlotManikins.Max(cs => cs.Modified))
                                        || (m.Course.CourseSlotActivities.Any() && m.EmailedUtc > m.Course.CourseSlotActivities.Max(cs => cs.Modified)))))
            })
        { }
    }
}
