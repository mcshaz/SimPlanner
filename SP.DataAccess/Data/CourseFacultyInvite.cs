namespace SP.DataAccess
{
    using Data.Interfaces;
    using Metadata;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseFacultyInvite : ICreated
    {
        public Guid CourseId { get; set; }
        public Guid ParticipantId { get; set; }
        public DateTime CreatedUtc { get; set; }

        [JsonIgnore]
        public virtual Course Course { get; set; }
        [JsonIgnore]
        public virtual Participant Faculty { get; set; }
    }
}
