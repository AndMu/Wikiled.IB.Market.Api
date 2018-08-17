namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsArticleMessage : IMessage
    {
        public NewsArticleMessage(int requestId, int articleType, string articleText)
        {
            RequestId = requestId;
            ArticleType = articleType;
            ArticleText = articleText;
        }

        public int ArticleType { get; }

        public string ArticleText { get; }
        public int RequestId { get; }
    }
}