using System;
using System.Collections.Generic;
namespace SM.Dto
{    
	public class CourseRoleTypeDto
	{
        public int Id { get; set; }
        public string Description { get; set; }
        public ICollection<CourseTypeDto> CourseTypes { get; set; }

        internal static Expression<Func<CourseRoleType, CourseRoleTypeDto>> map = m => new CourseRoleTypeDto
        {
            Id = m.Id,
            Description = m.Description,
            //CourseTypes = m.CourseTypes
        };
    }
}
