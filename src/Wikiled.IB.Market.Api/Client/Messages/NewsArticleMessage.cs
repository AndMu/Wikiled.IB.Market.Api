namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsArticleMessage
    {
        public int RequestId { get; private set; }

        public int ArticleType { get; private set; }

        public string ArticleText { get; private set; }

        public NewsArticleMessage(int requestId, int articleType, string articleText)
        {
            this.RequestId = requestId;
            this.ArticleType = articleType;
            this.ArticleText = articleText;
        }
    }
}
