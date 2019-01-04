namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickByTickBidAskMessage
    {
        public int ReqId { get; }

        public long Time { get; }

        public double BidPrice { get; }

        public double AskPrice { get; }

        public long BidSize { get; }

        public long AskSize { get; }

        public TickAttribBidAsk TickAttribBidAsk { get; }

        public TickByTickBidAskMessage(int reqId, long time, double bidPrice, double askPrice, long bidSize, long askSize, TickAttribBidAsk tickAttribBidAsk)
        {
            ReqId = reqId;
            Time = time;
            BidPrice = bidPrice;
            AskPrice = askPrice;
            BidSize = bidSize;
            AskSize = askSize;
            TickAttribBidAsk = tickAttribBidAsk;
        }
    }
}
