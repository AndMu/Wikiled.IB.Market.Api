using System;

namespace Wikiled.IB.Market.Api.Client.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ToUtc(this DateTime date, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeToUtc(date, timeZone);
        }
    }
}
