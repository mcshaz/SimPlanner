using SP.DataAccess;

using System.Linq;namespace SP.Dto.Maps
{
    internal class CourseDayMaps: DomainDtoMap<CourseDay, CourseDayDto>
        {
            return m => new CourseDay
            {
                CourseId = m.CourseId,
                Day = m.Day,
                Duration =m.Duration,
                StartUtc = m.Start
            };
        }
        /*
        internal static Expression<Func<Course, CourseDto>> MapFromDomain(string[] includes) {
            if (["Scenarios", "CourseType", "CourseParticipants", "ScenarioFacultyRoles"])
            {

            } */
        internal static Expression<Func<CourseDay, CourseDayDto>> MapFromDomain()
        {
            return m => new CourseDayDto
            {
                CourseId = m.CourseId,
                Day = m.Day,
                Duration = m.Duration,
                Start = m.StartUtc
            };
            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        }

        static void IsAllowed(string[] includes,params string[] allowed)
        {
            var disallowed = includes.Except(includes);
            if (disallowed.Any())
            {
                throw new ArgumentException(
                    string.Format("the include parameter(s){0} are not allowed: allowed parameters include ({1})",
                    string.Join(",", disallowed), string.Join(",", allowed)));
            }
        }

        /*
        internal static Expression<Func<Course, CourseDto>> mapBriefFromRepo = m => new CourseDto
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            CourseTypeId = m.CourseTypeId,

            CourseParticipants = m.CourseParticipants.Select(cp=>new CourseParticipantDto
            {
                ParticipantId = cp.ParticipantId,
                CourseId = cp.CourseId,
                IsConfirmed = cp.IsConfirmed,
                IsFaculty = cp.IsFaculty,
                DepartmentId = cp.DepartmentId,
                ProfessionalRoleId = cp.ProfessionalRoleId
            }).ToList()

            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,


            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
        */

    }
}
