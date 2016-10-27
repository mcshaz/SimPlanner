using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.DataAccess
{
    [MetadataType(typeof(ActivityResourceMetadata))]
    public class Activity : IAssociateFileOptional
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        
        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        public string FileName { get; set; }

        public Guid CourseActivityId { get; set; }
        public virtual CourseActivity CourseActivity { get; set; }
        public virtual ICollection<CourseSlotActivity> CourseSlotActivities { get; set; }
    }

    [MetadataType(typeof(ScenarioResourceMetadata))]
    public class ScenarioResource : IAssociateFileRequired
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime FileModified { get; set; }
        public long FileSize { get; set; }

        public Guid ScenarioId { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
