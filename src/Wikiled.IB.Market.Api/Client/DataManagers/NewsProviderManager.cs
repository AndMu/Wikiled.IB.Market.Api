using System;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class NewsProviderManager : SingleBaseDataManager<NewsProvidersMessage>
    {
        public NewsProviderManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.NewsProviders += OnMessage;
        }

        protected override int RequestOffset => 90020000;

        public IObservable<NewsProvidersMessage> Request()
        {
            var stream = Construct();
            IbClient.ClientSocket.ReqNewsProviders();
            return stream;
        }

        public override void Dispose()
        {
            IbClient.NewsProviders -= OnMessage;
            base.Dispose();
        }
    }
}