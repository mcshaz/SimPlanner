using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(CandidatePrereadingMetadata))]
    public class CandidatePrereadingDto : IAssociateFileRequired
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime FileModified { get; set; }
        public long FileSize { get; set; }

        public byte[] File { get; set; }

        public Guid CourseTypeId { get; set; }

        public CourseTypeDto CourseType { get; set; }
    }
}
