namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SessionParticipant")]
    public partial class SessionParticipant
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParticipantId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SessionId { get; set; }

        public bool Confirmed { get; set; }

        public bool Faculty { get; set; }

        public int DepartmentId { get; set; }

        public int ProfessionalRoleId { get; set; }

        public virtual Participant Participant { get; set; }

        public virtual Session Session { get; set; }
    }
}
