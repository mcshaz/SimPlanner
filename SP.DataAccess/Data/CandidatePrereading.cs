using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.DataAccess
{
    [MetadataType(typeof(CandidatePrereadingMetadata))]
    public class CandidatePrereading : IAssociateFileRequired
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime FileModified { get; set; }
        public long FileSize { get; set; }
        [NotMapped]
        public byte[] File { get; set; }

        public Guid CourseTypeId { get; set; }

        public virtual CourseType CourseType { get; set; }
    }
}
