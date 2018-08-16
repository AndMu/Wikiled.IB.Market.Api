namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsProvidersMessage
    {
        public NewsProvider[] NewsProviders { get; private set; }

        public NewsProvidersMessage(NewsProvider[] newsProviders)
        {
            this.NewsProviders = newsProviders;
        }
    }
}
