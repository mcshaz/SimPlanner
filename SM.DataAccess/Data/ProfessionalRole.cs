namespace SM.DataAccess
{
    using Enums;
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(ProfessionalRoleMetadata))]
    public class ProfessionalRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public ProfessionalCategory Category { get; set; }

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

		ICollection<Institution> _institutions; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institutions
		{
			get
			{
				return _institutions ?? (_institutions = new List<Institution>());
			}
			set
			{
				_institutions = value;
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
