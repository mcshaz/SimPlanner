namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SessionParticipant")]
    public partial class CourseParticipant
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ParticipantId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseId { get; set; }

        public bool Confirmed { get; set; }

        public bool Faculty { get; set; }

        public int? DepartmentId { get; set; }

        public int? ProfessionalRoleId { get; set; }

        public virtual Participant Participant { get; set; }

        public virtual Course Course { get; set; }
    }
}
