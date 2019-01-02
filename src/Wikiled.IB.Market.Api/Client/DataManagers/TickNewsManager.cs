using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class TickNewsManager : BaseDataManager<TickNewsMessage>
    {
        public TickNewsManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.TickNews += OnMessage;
        }

        protected override int RequestOffset => MessageIdConstants.TickNews;

        public IObservable<TickNewsMessage> Request(Contract contract)
        {
            var stream = Construct();
            IbClient.ClientSocket.ReqMktData(RequestOffset, contract, "mdoff,292", false, false, new List<TagValue>());
            return stream;
        }

        public override void Dispose()
        {
            IbClient.TickNews -= OnMessage;
            base.Dispose();
        }

        public override void Cancel()
        {
            IbClient.ClientSocket.CancelMktData(RequestId);
            base.Cancel();
        }
    }
}