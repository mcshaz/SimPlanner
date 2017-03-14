namespace SP.DataAccess
{
    using Enums;
    using SP.DataAccess.Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(CourseTypeMetadata))]
    public partial class CourseType : IAssociateFileOptional
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public Emersion? EmersionCategory { get; set; }
        public bool SendCandidateTimetable { get; set; }
        public string CertificateFileName { get; set; }

        public Guid? InstructorCourseId { get; set; }

        public virtual CourseType InstructorCourse { get; set; }

        public virtual ICollection<CourseActivity> CourseActivities { get; set; }
        public virtual ICollection<CourseTypeDepartment> CourseTypeDepartments { get; set; }
        public virtual ICollection<Scenario> Scenarios { get; set; }
        public virtual ICollection<CourseTypeScenarioRole> CourseTypeScenarioRoles { get; set;}
        public virtual ICollection<CourseFormat> CourseFormats { get; set; }
        public virtual ICollection<CandidatePrereading> CandidatePrereadings { get; set; }

        #region interfaceImplementation
        [NotMapped]
        public byte[] File { get; set; }
        [NotMapped]
        string IAssociateFile.FileName { get { return CertificateFileName; } set { CertificateFileName = value; } }

        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
        #endregion //interfaceImplementation
    }
}
