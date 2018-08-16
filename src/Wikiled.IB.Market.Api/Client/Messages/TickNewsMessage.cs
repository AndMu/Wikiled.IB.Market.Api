namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickNewsMessage
    {
        public int TickerId { get; }

        public long TimeStamp { get; }

        public string ProviderCode { get; }

        public string ArticleId { get; }
        
        public string Headline { get; }

        public string ExtraData { get; }

        public TickNewsMessage(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            TickerId = tickerId;
            TimeStamp = timeStamp;
            ProviderCode = providerCode;
            ArticleId = articleId;
            Headline = headline;
            ExtraData = extraData;
        }
    }
}
