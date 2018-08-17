using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Wikiled.IB.Market.Api.Client.Messages;

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

        protected override int RequestOffset => 30000000;

        public override void Dispose()
        {
            IbClient.HistoricalData -= OnMessage;
            IbClient.HistoricalDataUpdate -= OnMessage;
            IbClient.HistoricalDataEnd -= OnCompleted;
            base.Dispose();
        }

        public IObservable<HistoricalDataMessage> Request(Contract contract, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRth, int dateFormat, bool keepUpToDate)
        {
            Logger.LogDebug("Request: {0}", RequestId);
            Subject<HistoricalDataMessage> stream = Construct();
            IbClient.ClientSocket.ReqHistoricalData(RequestId, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRth, 1, keepUpToDate, new List<TagValue>());
            return stream;
        }
    }
}
