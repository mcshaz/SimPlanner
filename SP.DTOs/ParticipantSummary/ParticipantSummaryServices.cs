using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using SP.Dto.Utilities;
using System.Data.Entity;

namespace SP.Dto.ParticipantSummary
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
                                                && csp.CourseSlot.Activity != null
                                             group csp by csp.CourseSlot.Activity into a
                                             select new FacultyPresentationRoleSummary
                                             {
                                                 Description = a.Key.CourseType.Abbreviation + "-" + a.Key.Name,
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

        public static PriorExposures GetExposures(MedSimDbContext context ,Guid courseId, Guid userId)
        {
            //to do - check authorization
            var course = (from c in context.Courses.Include(co => co.CourseParticipants).Include(co => co.CourseFormat)
                          where c.Id == courseId
                          select c).First();
            //only interested in faculty
            var participantIds = (from cp in course.CourseParticipants
                                  where !cp.IsFaculty
                                  select cp.ParticipantId).ToList();
            //however, if they have been faculty for a scenario or activity, we want to know that
            var otherCourses = (from c in context.Courses.Include(co => co.CourseSlotActivities).Include(co => co.CourseParticipants)
                                where c.Id != courseId && c.CourseFormat.CourseTypeId == course.CourseFormat.CourseTypeId && !c.Cancelled && c.CourseParticipants.Any(cp => participantIds.Contains(cp.ParticipantId))
                                select c).ToList();
            var returnVar = new PriorExposures
            {
                ScenarioParticipants = new Dictionary<Guid, HashSet<Guid>>(),
                ActivityParticipants = new Dictionary<Guid, HashSet<Guid>>(),
                BookedManikins = GetBookedManikins(context, course)
            };

            foreach (var oc in otherCourses)
            {
                var participants = (from cp in oc.CourseParticipants
                                    select cp.ParticipantId).Intersect(participantIds).ToList();
                foreach (var csa in oc.CourseSlotActivities)
                {
                    if (csa.ActivityId.HasValue)
                    {
                        returnVar.ActivityParticipants.AddRangeOrCreate(csa.ActivityId.Value, participants);
                    }
                    else
                    {
                        returnVar.ScenarioParticipants.AddRangeOrCreate(csa.ScenarioId.Value, participants);
                    }
                }
            }
            return returnVar;
        }

        public static IEnumerable<KeyValuePair<Guid, string>> GetBookedManikins(MedSimDbContext context, Course course)
        {
            var refStart = course.StartUtc;
            var refFinish = course.FinishCourseUtc();
            return (from csm in context.CourseSlotManikins
                    let c = csm.Course
                    let lastDay = c.CourseDays.FirstOrDefault(cd=>cd.Day == c.CourseFormat.DaysDuration)
                    let cFinish = lastDay == null 
                        ? DbFunctions.AddMinutes(c.StartUtc,c.DurationMins)
                        : DbFunctions.AddMinutes(lastDay.StartUtc, lastDay.DurationMins)
                    where c.Id!= course.Id && c.StartUtc < refFinish &&  refStart < cFinish 
                    select new { csm.ManikinId, c.CourseFormat.Description, c.Department.Abbreviation })
                    .ToKeyValuePairList(a=>a.ManikinId, a=> a.Abbreviation + '-' + a.Description);
        }
    }
    public class PriorExposures
    {
        public Dictionary<Guid, HashSet<Guid>> ScenarioParticipants { get; set; }
        public Dictionary<Guid, HashSet<Guid>> ActivityParticipants { get; set; }
        public IEnumerable<KeyValuePair<Guid,string>> BookedManikins { get; set; }
    }
}
