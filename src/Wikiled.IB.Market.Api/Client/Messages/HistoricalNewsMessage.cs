namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalNewsMessage
    {
        public int RequestId { get; }

        public string Time { get; }

        public string ProviderCode { get; }

        public string ArticleId { get; }

        public string Headline { get; }

        public HistoricalNewsMessage(int requestId, string time, string providerCode, string articleId, string headline)
        {
            RequestId = requestId;
            Time = time;
            ProviderCode = providerCode;
            ArticleId = articleId;
            Headline = headline;
        }
    }
}
