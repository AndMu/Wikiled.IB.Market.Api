﻿using System;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class RealTimeBarMessage : HistoricalDataMessage
    {
        public long LongVolume { get; }

        public long Timestamp { get; }

        public DateTime TimeStamp
        {
            get
            {
                var start = new DateTime(1970, 1, 1, 0, 0, 0);
                return start.AddMilliseconds(Timestamp * 1000).ToLocalTime();
            }
        }

        public RealTimeBarMessage(int requestId, long date, double open, double high, double low, double close, long volume, double WAP, int count)
            : base(requestId, new Bar(UnixTimestampToDateTime(date).ToString("yyyyMMdd hh:mm:ss"), open, high, low, close, -1, count, WAP))
        {
            Timestamp = date;
            LongVolume = volume;
        }

        static DateTime UnixTimestampToDateTime(long unixTimestamp)
        {
            var unixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return unixBaseTime.AddSeconds(unixTimestamp);
        }
    }
}
