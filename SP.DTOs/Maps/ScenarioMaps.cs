using SP.DataAccess;
using SP.DataAccess.Enums;
using SP.Dto.ProcessBreezeRequests;
using System;
using System.Data.Entity;
using System.Linq;

namespace SP.Dto.Maps
{
    internal class ScenarioMaps: DomainDtoMap<Scenario, ScenarioDto>
    {
        public ScenarioMaps() : base(m => new Scenario
            {
                Id = m.Id,
                BriefDescription = m.BriefDescription,
                FullDescription =m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                CourseTypeId = m.CourseTypeId,
                Access = m.Access
            },
            m => new ScenarioDto
            {
                Id = m.Id,
                BriefDescription = m.BriefDescription,
                FullDescription = m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                CourseTypeId = m.CourseTypeId,
                Access = m.Access
            })
        {
            WherePredicate = v => {
                var isAdmin = v.AdminLevel != AdminLevels.None;
                return s => s.Access == SharingLevel.NoRestriction
                    || ((s.Access == SharingLevel.DepartmentOnly || s.Access == SharingLevel.DepartmentAndExParticipants) && s.DepartmentId == v.Principal.DefaultDepartmentId && isAdmin)
                    || (s.Access == SharingLevel.InstitutionWide && s.Department.InstitutionId == v.UserInstitutionId)
                    //todo - not accounting for the length of the last day - available 24 hours after start of last day
                    || (s.Access == SharingLevel.DepartmentAndExParticipants && s.CourseSlotScenarios.Any(css => DbFunctions.AddDays(css.Course.StartFacultyUtc,css.Course.CourseFormat.DaysDuration + 1) > DateTime.UtcNow && css.Course.CourseParticipants.Any(cp=>cp.ParticipantId == v.Principal.Id && cp.IsConfirmed == true)));
            };
        }
    }
}
