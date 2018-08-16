namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalNewsEndMessage
    {
        public int RequestId { get; private set; }
        public bool HasMore { get; private set; }

        public HistoricalNewsEndMessage(int requestId, bool hasMore)
        {
            this.RequestId = requestId;
            this.HasMore = hasMore;
        }
    }
}
