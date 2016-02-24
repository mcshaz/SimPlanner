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

		ICollection<Country> _countries; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Country> Countries
		{
			get
			{
				return _countries ?? (_countries = new List<Country>());
			}
			set
			{
				_countries = value;
			}
		}
    }
}
