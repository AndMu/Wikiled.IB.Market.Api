using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class HistoricalNewsManager : BaseDataManager<HistoricalNewsMessage>
    {
        public HistoricalNewsManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.HistoricalNews += OnMessage;
            ibClient.HistoricalNewsEnd += OnCompleted;
        }

        protected override int RequestOffset => MessageIdConstants.HistoricalNews;

        public IObservable<HistoricalNewsMessage> Request(int conId, string providerCodes, string startDateTime, string endDateTime, int totalResults)
        {
            var stream = Construct();
            IbClient.ClientSocket.ReqHistoricalNews(RequestId, conId, providerCodes, startDateTime, endDateTime, totalResults, new List<TagValue>());
            return stream;
        }

        public override void Dispose()
        {
            IbClient.HistoricalNews -= OnMessage;
            IbClient.HistoricalNewsEnd -= OnCompleted;
            base.Dispose();
        }
    }
}