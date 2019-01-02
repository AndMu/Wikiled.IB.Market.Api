using System;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client
{
    public interface IIBClient : IEWrapper
    {
        int ClientId { get; set; }

        EClientSocket ClientSocket { get; }

        int NextOrderId { get; set; }
        
        event Action<HistoricalTickMessage> historicalTick;
        event Action<HistoricalTickBidAskMessage> historicalTickBidAsk;
        event Action<HistoricalTickLastMessage> historicalTickLast;
        event Action<PnLMessage> pnl;
        event Action<PnLSingleMessage> pnlSingle;
        event Action<AdvisorDataMessage> ReceiveFA;
        event Action<TickByTickAllLastMessage> tickByTickAllLast;
        event Action<TickByTickBidAskMessage> tickByTickBidAsk;
        event Action<TickByTickMidPointMessage> tickByTickMidPoint;
        event Action<int, int, double, string, double, int, string, double, double> TickEFP;
        event Action<TickOptionMessage> TickOptionCommunication;
    }
}