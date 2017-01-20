namespace SP.DataAccess
{
    using Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(DepartmentMetadata))]
    public class Department: IAdminApproved
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Guid InstitutionId { get; set; }
        public string InvitationLetterFilename { get; set; }
        public string CertificateFilename { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
        public bool AdminApproved { get; set; }
        public DateTime CreatedUtc { get; set; }

        public virtual Institution Institution { get; set; }
        public virtual ICollection<CourseTypeDepartment> CourseTypeDepartments{ get; set; }
        public virtual ICollection<Manikin> Manikins { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Course> OutreachCourses { get; set; }
        public virtual ICollection<Scenario> Scenarios { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<CourseParticipant> CourseParticipants { get; set; }
    }
}
