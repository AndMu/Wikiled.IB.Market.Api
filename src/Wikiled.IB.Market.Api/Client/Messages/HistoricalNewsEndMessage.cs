namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalNewsEndMessage
    {
        public int RequestId { get; }
        public bool HasMore { get; }

        public HistoricalNewsEndMessage(int requestId, bool hasMore)
        {
            RequestId = requestId;
            HasMore = hasMore;
        }
    }
}
