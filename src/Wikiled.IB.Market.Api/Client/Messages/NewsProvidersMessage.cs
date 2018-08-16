namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsProvidersMessage
    {
        public NewsProvider[] NewsProviders { get; }

        public NewsProvidersMessage(NewsProvider[] newsProviders)
        {
            NewsProviders = newsProviders;
        }
    }
}
