namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickBidAskEndMessage
    {
        public int ReqId { get; }

        public HistoricalTickBidAskEndMessage(int reqId)
        {
            ReqId = reqId;
        }
    }
}
