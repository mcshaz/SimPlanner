using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{       
	public class CourseRoleTypeMetadata
	{
		[Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
