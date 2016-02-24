using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.DataAccess
{
    public class CourseSlotPresenter
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CourseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CourseSlotId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PresenterId { get; set; }

        public virtual Course Course { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
        public virtual Participant Presenter { get; set; }
    }
}
