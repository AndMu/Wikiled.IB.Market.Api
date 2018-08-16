namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickEndMessage
    {
        public int ReqId { get; }

        public HistoricalTickEndMessage(int reqId)
        {
            ReqId = reqId;
        }
    }
}
