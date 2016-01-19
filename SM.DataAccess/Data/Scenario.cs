namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Scenario")]
    public partial class Scenario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Description { get; set; }

        public int DepartmentId { get; set; }

        public Difficulty Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        [Required]
        [StringLength(256)]
        public string TemplateFilename { get; set; }

        public int? ManequinId { get; set; }

        public int SessionTypeId { get; set; }

        public virtual Manequin Manequin { get; set; }

        public virtual SessionType SessionType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> Resources { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioRoleType> Roles { get; set; }
    }
}
