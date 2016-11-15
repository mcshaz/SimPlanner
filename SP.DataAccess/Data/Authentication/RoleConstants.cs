using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SP.DataAccess
{
    public static class RoleConstants
    {
        public static readonly IReadOnlyDictionary<Guid, string> RoleNames;

        static RoleConstants()
        {
            const string idFormat = "D";
            RoleNames = new ReadOnlyDictionary<Guid, string>(new Dictionary<Guid, string> {
                [Guid.ParseExact("03fe7856-7b58-46b4-a1a5-1d70cf03bab2", idFormat)] = AccessAllData,
                [Guid.ParseExact("2adedaf3-b215-4cc7-8692-1a8e58584306", idFormat)] = AccessInstitution,
                [Guid.ParseExact("75a4d6c3-9e20-4567-8b49-5d791db8f110", idFormat)] = AccessDepartment,
                [Guid.ParseExact("e5fffe70-76ef-4cfd-8d58-089ba2198dc0", idFormat)] = SiteAdmin,
                [Guid.ParseExact("1a04cee2-f3f0-4965-a223-a2eae7937637", idFormat)] = DptRoomBookings,
                [Guid.ParseExact("e8149241-e33b-4510-a698-41b26b4b5d48", idFormat)] = DptManikinBookings,
            });
        }
        public const string SiteAdmin = "SiteAdmin";
        public const string AccessAllData = "AccessAllData";
        public const string AccessInstitution = "AccessInstitution";
        public const string AccessDepartment = "AccessDepartment";
        public const string DptRoomBookings = "DptRoomBookings";
        public const string DptManikinBookings = "DptManikinBookings";
        //public const string AdminApproved = "AdminApproved";

        //public const string AdminApprovedId = "9eb00f5d-a0cd-4a2d-a394-f9715bc203b6";
        //default = access courses you participate in only
    }
}
