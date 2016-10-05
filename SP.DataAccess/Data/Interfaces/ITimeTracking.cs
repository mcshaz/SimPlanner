using System;

namespace SP.DataAccess.Data.Interfaces
{
    internal interface ITimeTracking
    {
        DateTime CreatedUtc { get; set; }
        DateTime LastModifiedUtc { get; set; }
        bool SystemChangesOnly { get; set; }
    }
}
