using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.DataAccess
{
    public abstract class Resource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string ResourceFilename { get; set; }
    }
    public class CourseTeachingResource : Resource
    {
        public Guid CourseSlotId { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
    }

    public class ScenarioResource : Resource
    {
        public Guid ScenarioId { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
