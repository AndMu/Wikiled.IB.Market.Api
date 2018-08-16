namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickLastEndMessage
    {
        public int ReqId { get; }

        public HistoricalTickLastEndMessage(int reqId)
        {
            ReqId = reqId;
        }
    }
}
