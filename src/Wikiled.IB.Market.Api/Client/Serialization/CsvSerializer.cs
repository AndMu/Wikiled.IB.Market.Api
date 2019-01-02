using System;
using System.IO;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.Serialization
{
    public class CsvSerializer
    {
        private readonly IClientWrapper client;

        public CsvSerializer(IClientWrapper client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Save(string fileName, HistoricalDataMessage[] data)
        {
            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("Date,PX_OPEN,PX_HIGH,PX_LOW,PX_LAST,PX_VOLUME");
                for (var i = 0; i < data.Length; i++)
                {
                    HistoricalDataMessage item = data[i];
                    double high = item.High > (1.5 * item.Close) || item.High < item.Close ? item.Close : item.High;
                    double low = item.Low < (1.5 * item.Close) || item.Low > item.Close ? item.Close : item.Low;
                    double open = item.Open > high || item.Open < low ? i > 0 ? data[i - 1].Close : low : item.Open;
                    stream.WriteLine($"{item.Date.StrToDate(client.TimeZone):yyyy-MM-dd},{open},{high},{low},{item.Close},{item.Volume}");
                }
            }
        }
    }
}
