namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalNewsMessage
    {
        public int RequestId { get; private set; }

        public string Time { get; private set; }

        public string ProviderCode { get; private set; }

        public string ArticleId { get; private set; }

        public string Headline { get; private set; }

        public HistoricalNewsMessage(int requestId, string time, string providerCode, string articleId, string headline)
        {
            this.RequestId = requestId;
            this.Time = time;
            this.ProviderCode = providerCode;
            this.ArticleId = articleId;
            this.Headline = headline;
        }
    }
}
