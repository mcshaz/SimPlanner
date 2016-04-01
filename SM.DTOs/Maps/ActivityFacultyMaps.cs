using SM.DataAccess;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class ChosenTeachingResourceMaps
    {
        internal static Func<ChosenTeachingResourceDto, ChosenTeachingResource> mapToRepo()
        {
            return m => new ChosenTeachingResource
            {
                CourseId = m.CourseId,
                CourseSlotId =m.CourseSlotId,
                ActivityTeachingResourceId =m.ActivityTeachingResourceId
            };
        }

        internal static Expression<Func<ChosenTeachingResource, ChosenTeachingResourceDto>> mapFromRepo()
        {
            return m => new ChosenTeachingResourceDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ActivityTeachingResourceId = m.ActivityTeachingResourceId
                //Hospitals = m.Hospitals,
                //ProfessionalRoles = m.ProfessionalRoles
            };
        }
    }
}
