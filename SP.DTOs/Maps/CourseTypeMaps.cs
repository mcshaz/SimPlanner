using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class CourseTypeMaps: DomainDtoMap<CourseType, CourseTypeDto>
    {
        public CourseTypeMaps() : base(m => new CourseType
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId, 
                EmersionCategory = m.EmersionCategory, 
                SendCandidateTimetable = m.SendCandidateTimetable,
                CertificateFileName = m.CertificateFileName,
                FileModified = m.FileModified,
                FileSize = m.FileSize,
                File = m.File
            },
            m => new CourseTypeDto
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId,
                EmersionCategory = m.EmersionCategory,
                SendCandidateTimetable = m.SendCandidateTimetable,
                CertificateFileName = m.CertificateFileName,
                FileModified = m.FileModified,
                FileSize = m.FileSize
            })
        { }
    }
}
