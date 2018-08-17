namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class NewsProvidersMessage : IMessage
    {
        public NewsProvider[] NewsProviders { get; }

        public NewsProvidersMessage(NewsProvider[] newsProviders)
        {
            NewsProviders = newsProviders;
        }

        public int RequestId => 90020000;
    }
}
