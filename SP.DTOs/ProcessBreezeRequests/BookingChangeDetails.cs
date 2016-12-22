using SP.DataAccess;
using System.Collections.Generic;

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

    public class UserRequestingApproval
    {
        public Participant User { get; set; }
        public IEnumerable<Participant> Administrators { get; set; }
    }
}
