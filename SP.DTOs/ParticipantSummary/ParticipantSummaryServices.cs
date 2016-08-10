using SP.DataAccess;
using System;
using System.Linq;

namespace SP.DTOs.ParticipantSummary
{
    public static class ParticipantSummaryServices
    {
        public static ParticipantSummary GetParticipantSummary(MedSimDbContext context, Guid userId)
        {
            var returnVar = new ParticipantSummary();
            returnVar.CourseSummary = (from cp in context.CourseParticipants
                                       where cp.ParticipantId == userId
                                       group cp by cp.Course.CourseFormat into cf
                                       select new ParticipantCourseSummary
                                       {
                                           CourseName = cf.Key.CourseType.Abbreviation + " " + cf.Key.Description,
                                           FacultyCount = cf.Count(c=>c.IsFaculty),
                                           AtendeeCount = cf.Count(c=>!c.IsFaculty)
                                       }).ToList();
            returnVar.PresentationSummary = (from csp in context.CourseSlotPresenters
                                             where csp.ParticipantId == userId
                                             group csp by csp.CourseSlot.Activity into a
                                             select new FacultyPresentationRoleSummary
                                             {
                                                 Description = a.Key.Name,
                                                 Count = a.Count()
                                             }).ToList();

            returnVar.SimRoleSummary = (from cp in context.CourseScenarioFacultyRoles
                                     where cp.ParticipantId == userId
                                     group cp by cp.FacultyScenarioRole into fsr
                                     select new FacultySimRoleSummary
                                     {
                                         RoleName = fsr.Key.Description,
                                         Count = fsr.Count()
                                     }).ToList();
            return returnVar;
        }
    }
}
