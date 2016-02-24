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
        public Guid CourseId { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsFaculty { get; set; }

        public Guid? DepartmentId { get; set; }

        public Guid? ProfessionalRoleId { get; set; }

        public virtual Participant Participant { get; set; }

        public virtual Course Course { get; set; }
    }
}
