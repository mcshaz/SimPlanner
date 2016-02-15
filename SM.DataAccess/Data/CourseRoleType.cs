namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CourseRoleType
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

		ICollection<CourseType> _courseTypes 
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
