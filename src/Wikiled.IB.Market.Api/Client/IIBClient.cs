using System;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client
{
    public interface IIBClient : IEWrapper
    {
        int ClientId { get; set; }

        EClientSocket ClientSocket { get; }

        int NextOrderId { get; set; }
        
        event Action<HistoricalTickMessage> OnHistoricalTick;

        event Action<HistoricalTickBidAskMessage> OnHistoricalTickBidAsk;

        event Action<HistoricalTickLastMessage> OnHistoricalTickLast;

        event Action<PnLMessage> OnPnl;

        event Action<PnLSingleMessage> OnPnlSingle;

        event Action<AdvisorDataMessage> OnReceiveFA;

        event Action<TickByTickAllLastMessage> OnTickByTickAllLast;

        event Action<TickByTickBidAskMessage> OnTickByTickBidAsk;

        event Action<TickByTickMidPointMessage> OnTickByTickMidPoint;

        event Action<int, int, double, string, double, int, string, double, double> OnTickEFP;

        event Action<TickOptionMessage> OnTickOptionCommunication;
    }
}