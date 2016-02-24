namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Department")]
    public partial class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public Guid InstitutionId { get; set; }

        [StringLength(256)]
        public string InvitationLetterFilename { get; set; }

        [StringLength(256)]
        public string CertificateFilename { get; set; }

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

        public virtual Institution Institution { get; set; }

		ICollection<Manequin> _manequins; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manequin> Manequins
		{
			get
			{
				return _manequins ?? (_manequins = new List<Manequin>());
			}
			set
			{
				_manequins = value;
			}
		}

		ICollection<Course> _courses; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses
		{
			get
			{
				return _courses ?? (_courses = new List<Course>());
			}
			set
			{
				_courses = value;
			}
		}

        ICollection<Scenario> _scenrios;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
        {
            get
            {
                return _scenrios ?? (_scenrios = new List<Scenario>());
            }
            set
            {
                _scenrios = value;
            }
        }
    }
}
