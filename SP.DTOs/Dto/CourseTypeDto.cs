namespace SP.Dto
{
    using DataAccess.Enums;
    using SP.DataAccess.Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(CourseTypeMetadata))]
    public partial class CourseTypeDto : IAssociateFileOptional
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public Emersion? EmersionCategory { get; set; }
        public Guid? InstructorCourseId { get; set; }
        public bool SendCandidateTimetable { get; set; }
        public string CertificateFileName { get; set; }

        public virtual CourseTypeDto InstructorCourse { get; set; }

        public ICollection<CourseActivityDto> CourseActivities { get; set; }
        public ICollection<CourseTypeDepartmentDto> CourseTypeDepartments { get; set; }
        public ICollection<ScenarioDto> Scenarios { get; set; }
        public ICollection<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles { get; set; }
        public ICollection<CourseFormatDto> CourseFormats { get; set; }

        public virtual ICollection<CandidatePrereadingDto> CandidatePrereadings { get; set; }

        #region interfaceImplementation
        [NotMapped]
        string IAssociateFile.FileName { get { return CertificateFileName; } set { CertificateFileName = value; } }

        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        public byte[] File { get; set; }
        #endregion
    }
}
