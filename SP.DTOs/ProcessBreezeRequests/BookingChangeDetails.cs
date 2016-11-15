using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Dto.ProcessBreezeRequests
{
    public class BookingChangeDetails
    {
        public IEnumerable<Participant> Notify { get; set; }
        public Participant PersonBooking { get; set; }
        public Course RelevantCourse { get; set; }
        public Room AddedRoomBooking { get; set; }
        public Room RemovedRoomBooking { get; set; }
        public IEnumerable<Manikin> AddedManikinBookings { get; set; }
        public IEnumerable<Manikin> RemovedManikinBookings { get; set; }
    }
}
