namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class ScenarioRoleDescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } 

        [Required]
        [StringLength(50)]
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

        ICollection<ScenarioFacultyRole> _scenarioFacultyRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioFacultyRole> ScenarioFacultyRoles
        {
            get
            {
                return _scenarioFacultyRoles ?? (_scenarioFacultyRoles = new List<ScenarioFacultyRole>());
            }
            set
            {
                _scenarioFacultyRoles = value;
            }
        }
    }
}
