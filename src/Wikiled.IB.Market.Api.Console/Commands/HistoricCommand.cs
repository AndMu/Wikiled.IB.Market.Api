using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Request;
using Wikiled.IB.Market.Api.Client.Serialization;
using Wikiled.IB.Market.Api.Client.Types;
using Wikiled.IB.Market.Api.Console.Commands.Config;

namespace Wikiled.IB.Market.Api.Console.Commands
{
    /// <summary>
    /// historic -Stock=VXX
    /// </summary>
    public class HistoricCommand : Command
    {
        private readonly ILogger<HistoricCommand> log;

        private readonly HistoricConfig config;

        private readonly IClientWrapper client;

        private readonly HistoricalDataManager historicalDataManager;

        private readonly ICsvSerializer serializer;

        public HistoricCommand(ILogger<HistoricCommand> log,
                               IClientWrapper client,
                               HistoricalDataManager historicalDataManager,
                               HistoricConfig config,
                               ICsvSerializer serializer)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.historicalDataManager =
                historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        protected override async Task Execute(CancellationToken token)
        {
            log.LogInformation("Starting...");
            if (!client.Connect())
            {
                log.LogError("Connection failed");
                return;
            }

            var request = historicalDataManager
                            .Request(
                                new MarketDataRequest(
                                    ContractHelper.GetContract(config.Stock),
                                    DateTime.UtcNow.Date,
                                    new Duration(5, DurationType.Years),
                                    BarSize.Day,
                                    WhatToShow.BID_ASK));
            await serializer.Save($"{config.Stock}_historic.csv", request, token).ConfigureAwait(false);
            log.LogInformation("History request completed");
        }
    }
}
