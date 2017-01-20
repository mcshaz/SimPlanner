using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Dto.Utilities
{
    public sealed class DualTimetable
    {
        public MemoryStream ParticipantTimetable { get; set; }
        public MemoryStream FacultyTimetable { get; set; }
    }
}
