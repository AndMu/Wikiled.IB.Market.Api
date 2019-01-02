using System;
using System.Reactive.Linq;
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
    /// historic -Stock=VXX -Out=price.csv
    /// </summary>
    public class HistoricCommand : Command
    {
        private readonly ILogger<HistoricCommand> log;

        private readonly HistoricConfig config;

        private readonly IClientWrapper client;

        private readonly HistoricalDataManager historicalDataManager;

        public HistoricCommand(ILogger<HistoricCommand> log, IClientWrapper client, HistoricalDataManager historicalDataManager, HistoricConfig config)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.historicalDataManager = historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
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
                                    GetContract(config.Stock),
                                    new DateTime(2016, 01, 01).ToUtc(client.TimeZone),
                                    new Duration(5, DurationType.Years),
                                    BarSize.Day,
                                    WhatToShow.ASK));
            var data = await request.ToArray();
            var serializer = new CsvSerializer(client);
            serializer.Save(config.Stock, data);
        }

        private static Contract GetContract(string stock)
        {
            var contract = new Contract
                           {
                               SecType = SecType.STK,
                               Symbol = stock,
                               Exchange = ExchangeType.ISLAND,
                               Currency = "USD"
                           };

            return contract;
        }
    }
}
