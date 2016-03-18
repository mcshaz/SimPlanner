namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(DepartmentMetadata))]
    public class Department
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public Guid InstitutionId { get; set; }

        public string InvitationLetterFilename { get; set; }

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

        ICollection<Course> _outreachCourses;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> OutreachCourses
        {
            get
            {
                return _outreachCourses ?? (_outreachCourses = new List<Course>());
            }
            set
            {
                _outreachCourses = value;
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

        ICollection<Participant> _participants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant> Participants
        {
            get
            {
                return _participants ?? (_participants = new List<Participant>());
            }
            set
            {
                _participants = value;
            }
        }

        ICollection<Room> _rooms;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Room> Rooms
        {
            get
            {
                return _rooms ?? (_rooms = new List<Room>());
            }
            set
            {
                _rooms = value;
            }
        }

        ICollection<CourseParticipant> _courseParticipants;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> CourseParticipants
        {
            get
            {
                return _courseParticipants ?? (_courseParticipants = new List<CourseParticipant>());
            }
            set
            {
                _courseParticipants = value;
            }
        }
    }
}
