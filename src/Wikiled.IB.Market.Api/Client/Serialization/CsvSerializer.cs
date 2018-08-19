using System;
using System.IO;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.Serialization
{
    public class CsvSerializer
    {
        private readonly IBClientWrapper client;

        public CsvSerializer(IBClientWrapper client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Save(string fileName, HistoricalDataMessage[] data)
        {
            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("Date,PX_OPEN,PX_HIGH,PX_LOW,PX_LAST,PX_VOLUME");
                foreach (var item in data)
                {
                    stream.WriteLine($"{item.Date.StrToDate(client.TimeZone):yyyy-MM-dd},{item.Open},{item.High},{item.Low},{item.Close},{item.Volume}");
                }
            }
        }
    }
}
