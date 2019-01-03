using System;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.Serialization
{
    public class CsvSerializer : ICsvSerializer
    {
        private readonly IClientWrapper client;

        public CsvSerializer(IClientWrapper client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task Save(string fileName, IObservable<IPriceData> data, CancellationToken token)
        {
            using (var streamOutCsv = new StreamWriter(fileName, false, Encoding.UTF8))
            using (var csvDataOut = new CsvWriter(streamOutCsv))
            {
                csvDataOut.WriteField("Date");
                csvDataOut.WriteField("PX_OPEN");
                csvDataOut.WriteField("PX_HIGH");
                csvDataOut.WriteField("PX_LOW");
                csvDataOut.WriteField("PX_LAST");
                csvDataOut.WriteField("PX_VOLUME");
                csvDataOut.NextRecord();
                IPriceData previous = null;
                await data.ForEachAsync(item => { previous = WritePrice(item, previous, csvDataOut); }, token).ConfigureAwait(false);
            }
        }

        private IPriceData WritePrice(IPriceData item, IPriceData previous, CsvWriter csvDataOut)
        {
            double high = item.High > (1.5 * item.Close) || item.High < item.Close
                ? item.Close
                : item.High;
            double low = item.Low < (1.5 * item.Close) || item.Low > item.Close
                ? item.Close
                : item.Low;
            double open = item.Open > high || item.Open < low
                ? previous?.Close ?? low
                : item.Open;
            previous = item;
            csvDataOut.WriteField(
                item.Time.StrToDate(client.TimeZone).ToString("yyyy-MM-dd"));
            csvDataOut.WriteField(open);
            csvDataOut.WriteField(high);
            csvDataOut.WriteField(low);
            csvDataOut.WriteField(item.Close);
            csvDataOut.WriteField(item.Volume);
            csvDataOut.NextRecord();
            return previous;
        }
    }
}
