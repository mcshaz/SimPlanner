namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [MetadataType(typeof(InstructorCourseMetadata))]
    public class InstructorCourse
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public string Description { get; set; }

        public int? MaximumParticipants { get; set; }

        ICollection<InstructorCourseParticipant> _instructorCourseParticipants; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstructorCourseParticipant> InstructorCourseParticipants
		{
			get
			{
				return _instructorCourseParticipants ?? (_instructorCourseParticipants = new List<InstructorCourseParticipant>());
			}
			set
			{
				_instructorCourseParticipants = value;
			}
		}
    }
}
