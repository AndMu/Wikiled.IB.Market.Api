using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Messages;
using Wikiled.IB.Market.Api.Client.Request;
using Wikiled.IB.Market.Api.Client.Serialization;
using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //configure NLog
            LoggerFactory factory = new LoggerFactory();
            factory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            using (IBClientWrapper client = new IBClientWrapper(factory))
            {
                client.Connect("127.0.0.1", 7496, 1);
                IObservable<HistoricalDataMessage> amd = client.GetManager<HistoricalDataManager>()
                                .Request(
                                    new MarketDataRequest(
                                        GetMDContract("VXX"),
                                        new DateTime(2016, 01, 01).ToUtc(client.TimeZone),
                                        new Duration(5, DurationType.Years),
                                        BarSize.Day,
                                        WhatToShow.ASK));
                HistoricalDataMessage[] data = await amd.ToArray();
                var date = data[0].Date.StrToDate(client.TimeZone);
                var serializer = new CsvSerializer(client);
                serializer.Save("price.csv", data);
                System.Console.ReadLine();
            }

            NLog.LogManager.Shutdown();
        }

      

        private void Old()
        {
        //    IObservable<HistoricalDataMessage> amd = client.GetManager<HistoricalDataManager>()
        //                                                   .Request(
        //                                                       new MarketDataRequest(
        //                                                           GetMDContract("SVXY"),
        //                                                           new DateTime(2013, 08, 08, 23, 59, 59, DateTimeKind.Utc),
        //                                                           new Duration(2, DurationType.Days),
        //                                                           BarSize.Min,
        //                                                           WhatToShow.MIDPOINT));
        //    HistoricalDataMessage[] data = await amd.ToArray();

            //var realTime = client.GetManager<RealTimeBarsManager>();
            //var providers = await client.GetManager<NewsProviderManager>().Request().FirstOrDefaultAsync();
            //var contract = await client.GetManager<ContractManager>().Request(GetMDContract("AMD")).FirstOrDefaultAsync();
            //var news = client.GetManager<HistoricalNewsManager>()
            //                 .Request(contract.ContractDetails.Contract.ConId, providers.NewsProviders[2].ProviderCode, "20170708 23:59:59 GMT", "20180808 23:59:59 GMT", 100);
            //var newsData = await news.ToArray();
            //var stream = realTime.Request(GetMDContract("AMD"), WhatToShow.BID);
            //var subscription = stream.Subscribe(item => { System.Console.WriteLine(item.Close); });
            //await Task.Delay(50000).ConfigureAwait(false);
            //realTime.Cancel();
            //subscription.Dispose();
        }

        private static Contract GetMDContract(string stock)
        {
            Contract contract = new Contract
            {
                SecType = SecType.STK,
                Symbol = stock,
                Exchange = ExchangeType.ISLAND,
                Currency = "USD"
            };
            //contract.LastTradeDateOrContractMonth = null;
            //contract.PrimaryExch = null;
            //contract.IncludeExpired = includeExpired.Checked;
            //if (!mdContractRight.Text.Equals("") && !mdContractRight.Text.Equals("None"))
            //{
            //    contract.Right = (string)((IBType)mdContractRight.SelectedItem).Value;
            //}
            //contract.Strike = stringToDouble(this.strike_TMD_MDT.Text);
            //contract.Multiplier = this.multiplier_TMD_MDT.Text;
            //contract.LocalSymbol = this.localSymbol_TMD_MDT.Text;

            return contract;
        }
    }
}
