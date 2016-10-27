using System;

namespace SP.DataAccess.Utilities
{
    internal static class DateTimeExtensions
    {
        internal static DateTime AsUtc(this DateTime inptDate)
        {
            switch (inptDate.Kind)
            {
                case DateTimeKind.Local:
                    throw new ArgumentException("DateTime MUST NOT be of kind local");
                case DateTimeKind.Utc:
                    return inptDate;
                default:
                    return DateTime.SpecifyKind(inptDate, DateTimeKind.Utc);
            }
        }
    }
}
