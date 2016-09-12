using SP.DataAccess;
using System;
using System.Data.SqlTypes;
using System.Linq;

namespace SP.DTOs.ParticipantSummary
{
    public static class ParticipantSummaryServices
    {
        public static ParticipantSummary GetParticipantSummary(MedSimDbContext context, Guid userId, DateTime? after=null)
        {
            var returnVar = new ParticipantSummary();
            if (!after.HasValue) { after = SqlDateTime.MinValue.Value; }
            returnVar.CourseSummary = (from cp in context.CourseParticipants
                                       let course = cp.Course
                                       where cp.ParticipantId == userId && !course.Cancelled && course.StartUtc >= after
                                       group cp by course.CourseFormat into cf
                                       select new ParticipantCourseSummary
                                       {
                                           CourseName = cf.Key.CourseType.Abbreviation + " " + cf.Key.Description,
                                           FacultyCount = cf.Count(c=>c.IsFaculty),
                                           AtendeeCount = cf.Count(c=>!c.IsFaculty)
                                       }).ToList();

            returnVar.PresentationSummary = (from csp in context.CourseSlotPresenters
                                             let course = csp.Course
                                             where csp.ParticipantId == userId && !course.Cancelled && course.StartUtc >= after
                                             group csp by csp.CourseSlot.Activity into a
                                             select new FacultyPresentationRoleSummary
                                             {
                                                 Description = a.Key.Name,
                                                 Count = a.Count()
                                             }).ToList();

            returnVar.SimRoleSummary = ( from csfr in context.CourseScenarioFacultyRoles
                                         let course = csfr.Course
                                         where csfr.ParticipantId == userId && !course.Cancelled && course.StartUtc >= after
                                         group csfr by csfr.FacultyScenarioRole into fsr
                                         select new FacultySimRoleSummary
                                         {
                                             RoleName = fsr.Key.Description,
                                             Count = fsr.Count()
                                         }).ToList();
            return returnVar;
        }
    }
}
