namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseRoleTypeMetadata))]
    public class CourseRoleType
    {
        [Key]
        public int Id { get; set; } 

        public string Description { get; set; }

        ICollection<CourseType> _courseTypes; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseType> CourseTypes
		{
			get
			{
				return _courseTypes ?? (_courseTypes = new List<CourseType>());
			}
			set
			{
				_courseTypes = value;
			}
		}
    }
}
