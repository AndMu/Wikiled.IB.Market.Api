using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;
using Wikiled.IB.Market.Api.Client.Messages;
using Wikiled.IB.Market.Api.Client.Request;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class HistoricalDataManager : BaseDataManager<HistoricalDataMessage>
    {
        public HistoricalDataManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.HistoricalData += OnMessage;
            ibClient.HistoricalDataUpdate += OnMessage;
            ibClient.HistoricalDataEnd += OnCompleted;
        }

        protected override int RequestOffset => MessageIdConstants.HistoricalData;

        public override void Dispose()
        {
            IbClient.HistoricalData -= OnMessage;
            IbClient.HistoricalDataUpdate -= OnMessage;
            IbClient.HistoricalDataEnd -= OnCompleted;
            base.Dispose();
        }

        public IObservable<HistoricalDataMessage> Request(MarketDataRequest request)
        {
            Logger.LogDebug("Request: {0}", RequestId);
            var stream = Construct();
            IbClient.ClientSocket.ReqHistoricalData(RequestId, request);
            return stream;
        }
    }
}
