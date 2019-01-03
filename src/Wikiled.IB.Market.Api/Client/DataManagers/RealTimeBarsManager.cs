using System;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;
using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class RealTimeBarsManager : BaseDataManager<RealTimeBarMessage>
    {
        public RealTimeBarsManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.RealtimeBar += OnMessage;
        }

        protected override int RequestOffset => MessageIdConstants.RealTime;

        public override void Dispose()
        {
            IbClient.RealtimeBar -= OnMessage;
            base.Dispose();
        }

        public IObservable<RealTimeBarMessage> Request(Contract contract, WhatToShow whatToShow, bool useRth = false)
        {
            var stream = Construct();
            IbClient.ClientSocket.ReqRealTimeBars(RequestId, contract, 5, whatToShow, useRth, null);
            return stream;
        }

        public override void Cancel()
        {
            IbClient.ClientSocket.CancelRealTimeBars(RequestId);
            base.Cancel();
        }
    }
}