namespace SM.DataAccess
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ScenarioResource")]
    public partial class ScenarioResource
    {
        [Key]
        public int Id { get; set; }

        public int ScenarioTypeId { get; set; }

        [Required]
        [StringLength(256)]
        public string ResourceFilename { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        private ICollection<Scenario> _scenarios;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
        {
            get { return _scenarios ?? (_scenarios = new List<Scenario>()): }
            set { _scenarios = value; }
        }
    }
}
