using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Console.Commands.Config;

namespace Wikiled.IB.Market.Api.Console.Commands
{
    /// <summary>
    /// news -Symbol=VXX
    /// </summary>
    public class NewsCommand : Command
    {
        private readonly ILogger<NewsCommand> log;

        private readonly NewsConfig config;

        private readonly IClientWrapper client;

        private readonly NewsProviderManager newsProvider;

        private readonly ContractManager contractManager;

        private readonly HistoricalNewsManager newsManager;

        public NewsCommand(ILogger<NewsCommand> log,
                           IClientWrapper client,
                           NewsConfig config,
                           NewsProviderManager newsProvider,
                           ContractManager contractManager,
                           HistoricalNewsManager newsManager)
            : base(log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.newsProvider = newsProvider ?? throw new ArgumentNullException(nameof(newsProvider));
            this.contractManager = contractManager ?? throw new ArgumentNullException(nameof(contractManager));
            this.newsManager = newsManager ?? throw new ArgumentNullException(nameof(newsManager));
        }

        protected override async Task Execute(CancellationToken token)
        {
            log.LogInformation("Starting...");
            if (!client.Connect())
            {
                log.LogError("Connection failed");
                return;
            }

            var providers = await newsProvider.Request().FirstOrDefaultAsync();
            var contract = await contractManager.Request(ContractHelper.GetStockContract(config.Symbol)).FirstOrDefaultAsync();
            var news = newsManager.Request(contract.ContractDetails.Contract.ConId,
                                           providers.NewsProviders[2].ProviderCode,
                                           StringFormater.StrToDate(config.From, client.TimeZone).DateToStr(),
                                           StringFormater.StrToDate(config.To, client.TimeZone).DateToStr(),
                                           100);
            var newsData = await news.ToArray();
            log.LogInformation("News request completed");
        }
    }
}
