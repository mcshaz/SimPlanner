using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimManager.Tests
{
    internal class Foo
    {
        [Key]
        public int FooId { get; set; }
        [ForeignKey("B")]
        public int BId { get; set; }
        public Bar B { get; set; }
        public virtual ICollection<Bar> Bars { get; set; }
    }

    internal class FooDto
    {
        [Key]
        public int FooId { get; set; }
        [ForeignKey("B")]
        public int BId { get; set; }
        public BarDto B { get; set; }
        public virtual ICollection<BarDto> Bars { get; set; }
    }

    internal class Bar
    {
        [Key]
        public int BarId { get; set; }
        public string BarString { get; set; }
        [ForeignKey("F")]
        public int FId { get; set; }
        public Foo F { get; set; }
        public virtual ICollection<Foo> Foos { get; set; }
    }

    internal class BarDto
    {
        [Key]
        public int BarId { get; set; }
        public string BarString { get; set; }
        [ForeignKey("F")]
        public int FId { get; set; }
        public FooDto F { get; set; }
        public virtual ICollection<FooDto> Foos { get; set; }
    }
}
