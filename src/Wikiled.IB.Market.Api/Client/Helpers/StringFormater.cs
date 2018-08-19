using System;
using System.Globalization;
using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Client.Helpers
{
    public static class StringFormater
    {
        public static string DateToStr(this DateTime date)
        {
            //"20130808 23:59:59 GMT"
            return date.ToUniversalTime().ToString("yyyyMMdd HH:mm:ss") + " GMT";
        }

        public static DateTime StrToDate(this string date, TimeZoneInfo timeZone)
        {
            //"20130808 23:59:59 GMT"
            if (DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            if (!DateTime.TryParseExact(date, "yyyyMMdd  HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                throw new ArgumentOutOfRangeException(nameof(date));
            }

            return TimeZoneInfo.ConvertTimeToUtc(result, timeZone);
        }

        public static string BarToStr(this BarSize size)
        {
            switch (size)
            {
                case BarSize.Sec:
                    return "1 sec";
                case BarSize.FiveSecs:
                    return "5 sec";
                case BarSize.FiveTeenSecs:
                    return "15 sec";
                case BarSize.ThirtySecs:
                    return "30 sec";
                case BarSize.Min:
                    return "1 min";
                case BarSize.TwoMins:
                    return "2 min";
                case BarSize.ThreeMins:
                    return "3 min";
                case BarSize.FiveMins:
                    return "5 min";
                case BarSize.FiveteenMins:
                    return "15 min";
                case BarSize.ThirtyMins:
                    return "30 min";
                case BarSize.Hour:
                    return "1 hour";
                case BarSize.Day:
                    return "1 day";
                case BarSize.Week:
                    return "1 week";
                case BarSize.Month:
                    return "1 month";
                default:
                    throw new ArgumentOutOfRangeException(nameof(size));
            }
        }
    }
}