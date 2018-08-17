using System;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class RealTimeBarsManager : BaseDataManager
    {
        public const int RT_BARS_ID_BASE = 40000000;

        private readonly ILogger logger;

        public RealTimeBarsManager(IBClient ibClient, ILoggerFactory loggerFactory) 
            : base(ibClient)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<RealTimeBarsManager>();
        }

        //public IObservable<HistoricalDataMessage> Request(Contract contract, string whatToShow, bool useRTH)
        //{
        //    IbClient.ClientSocket.CancelRealTimeBars(CurrentTicker + RT_BARS_ID_BASE);
        //    IbClient.ClientSocket.ReqRealTimeBars(CurrentTicker + RT_BARS_ID_BASE, contract, 5, whatToShow, useRTH, null);
        //}

        //public void UpdateUI(RealTimeBarMessage rtBar)
        //{
        //    BarCounter++;
        //    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0);
        //    DateTime dt = start.AddMilliseconds(rtBar.Timestamp * 1000).ToLocalTime();
        //}
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
