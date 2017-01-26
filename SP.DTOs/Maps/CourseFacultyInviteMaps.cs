using SP.DataAccess;
using System;
using System.Data.SqlTypes;
using System.Linq;

namespace SP.Dto.Maps
{
    public class CourseFacultyInviteMaps: DomainDtoMap<CourseFacultyInvite, CourseFacultyInviteDto>
    {
        public CourseFacultyInviteMaps(): base(m => new CourseFacultyInvite
        {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId
            }, 
            m => new CourseFacultyInviteDto
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId
            })
        { }
    }
}
