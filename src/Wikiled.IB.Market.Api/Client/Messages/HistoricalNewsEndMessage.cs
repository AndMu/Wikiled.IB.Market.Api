namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalNewsEndMessage : IMessage
    {
        public HistoricalNewsEndMessage(int requestId, bool hasMore)
        {
            RequestId = requestId;
            HasMore = hasMore;
        }

        public bool HasMore { get; }
        public int RequestId { get; }
    }
}