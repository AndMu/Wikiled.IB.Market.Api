using System;
using System.Collections.Generic;
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

        private readonly Func<HistoricalDataManager> historicalDataManager;

        private readonly ICsvSerializer serializer;

        public OptionHistoryCommand(
            ILogger<OptionHistoryCommand> log,
            IClientWrapper client,
            Func<HistoricalDataManager> historicalDataManager,
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

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                double strike = config.Strike + i * 10;
                var request = historicalDataManager()
                    .Request(
                        new MarketDataRequest(
                            ContractHelper.GetOptionsContract(config.Symbol, strike, config.Expiry, OptionType.CALL),
                            DateTime.UtcNow.Date,
                            new Duration(5, DurationType.Months),
                            BarSize.Hour,
                            WhatToShow.MIDPOINT));
                var task = serializer.Save($"{config.Symbol}_{config.Expiry}_{config.Type}_{strike}_historic.csv", request, token);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            log.LogInformation("History request completed");
        }
    }
}
