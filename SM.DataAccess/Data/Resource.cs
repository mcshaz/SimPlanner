using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DataAccess
{
    public abstract class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string ResourceFilename { get; set; }
    }
    public class CourseTeachingResource : Resource
    {
        public int CourseSlotId { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
    }

    public class ScenarioResource : Resource
    {
        public int ScenarioId { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
