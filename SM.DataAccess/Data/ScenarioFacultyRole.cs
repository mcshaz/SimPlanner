using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.DataAccess
{
    public class ScenarioFacultyRole
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CourseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ScenarioId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid FacultyMemberId { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RoleId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Scenario Scenario{ get; set; }
        public virtual ScenarioRoleDescription Role { get; set; }
        public virtual Participant FacultyMember { get; set; }
    }
}
