using SP.DataAccess;

using System.Linq;
namespace SP.Dto.Maps
{
    internal class RoomMaps: DomainDtoMap<Room, RoomDto>
    {
        public RoomMaps() : base(m => new Room
            {
                Id = m.Id,
                DepartmentId = m.DepartmentId,
                ShortDescription = m.ShortDescription,
                FullDescription = m.FullDescription,
                Directions = m.Directions,
                FileModified = m.FileModified,
                FileName = m.FileName,
                FileSize = m.FileSize,
                File = m.File
            },
            m => new RoomDto
            {
                Id = m.Id,
                DepartmentId = m.DepartmentId,
                ShortDescription = m.ShortDescription,
                FullDescription = m.FullDescription,
                Directions = m.Directions,
                FileModified = m.FileModified,
                FileName = m.FileName,
                FileSize = m.FileSize
            })
        { }
    }
}
