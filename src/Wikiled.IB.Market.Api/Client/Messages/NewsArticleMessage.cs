namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsArticleMessage
    {
        public int RequestId { get; }

        public int ArticleType { get; }

        public string ArticleText { get; }

        public NewsArticleMessage(int requestId, int articleType, string articleText)
        {
            RequestId = requestId;
            ArticleType = articleType;
            ArticleText = articleText;
        }
    }
}
