namespace SP.DataAccess
{
    using Enums;
    using SP.Metadata;
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

        public int Order { get; set; }


        public virtual ICollection<Participant> Participants
		{
            get; set;
		}

        public virtual ICollection<ProfessionalRoleInstitution> ProfessionalRoleInstitutions { get; set; }


        public virtual ICollection<CourseParticipant> CourseParticipants
        {
            get; set;
        }
    }
}
