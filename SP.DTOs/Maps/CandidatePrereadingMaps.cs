using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class CandidatePrereadingMaps: DomainDtoMap<CandidatePrereading, CandidatePrereadingDto>
    {
        public CandidatePrereadingMaps() : base(m => new CandidatePrereading
        {
                Id = m.Id, 
                CourseTypeId = m.CourseTypeId,
                FileName = m.FileName,
                FileModified = m.FileModified,
                FileSize = m.FileSize
            },
            m => new CandidatePrereadingDto
            {
                Id = m.Id,
                CourseTypeId = m.CourseTypeId,
                FileName = m.FileName,
                FileModified = m.FileModified,
                FileSize = m.FileSize
            })
        { }
    }
}
