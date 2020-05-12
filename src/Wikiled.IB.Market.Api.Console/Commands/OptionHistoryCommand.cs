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
    /// options -Symbol=MSFT -Strike=150 -Expiry=20200619 -Type=Call
    /// </summary>
    public class OptionHistoryCommand : Command
    {
        private readonly ILogger<OptionHistoryCommand> log;

        private readonly OptionsHistoricConfig config;

        private readonly IClientWrapper client;

        private readonly HistoricalDataManager historicalDataManager;

        private readonly ICsvSerializer serializer;

        public OptionHistoryCommand(
            ILogger<OptionHistoryCommand> log,
            IClientWrapper client,
            HistoricalDataManager historicalDataManager,
            OptionsHistoricConfig config,
            ICsvSerializer serializer)
            : base(log)
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
                                    ContractHelper.GetOptionsContract(config.Symbol, config.Strike, config.Expiry, ),
                                    DateTime.UtcNow.Date,
                                    new Duration(5, DurationType.Months),
                                    BarSize.Hour,
                                    WhatToShow.MIDPOINT));
            await serializer.Save($"{config.Symbol}_{config.Expiry}_{config.Type}_{config.Strike}_historic.csv", request, token).ConfigureAwait(false);
            log.LogInformation("History request completed");
        }
    }
}
