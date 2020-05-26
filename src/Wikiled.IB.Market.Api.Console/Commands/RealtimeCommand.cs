using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;
using Wikiled.IB.Market.Api.Client.Helpers;
using Wikiled.IB.Market.Api.Client.Serialization;
using Wikiled.IB.Market.Api.Client.Types;
using Wikiled.IB.Market.Api.Console.Commands.Config;

namespace Wikiled.IB.Market.Api.Console.Commands
{
    /// <summary>
    /// realtime -Symbol=VXX
    /// </summary>
    public class RealtimeCommand : Command
    {
        private readonly ILogger<RealtimeCommand> log;

        private readonly RealtimeConfig config;

        private readonly IClientWrapper client;

        private readonly RealTimeBarsManager realTimeBarsManager;

        private readonly ICsvSerializer serializer;

        public RealtimeCommand(ILogger<RealtimeCommand> log,
                               IClientWrapper client,
                               RealTimeBarsManager realTimeBarsManager,
                               RealtimeConfig config,
                               ICsvSerializer serializer)
            : base(log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.realTimeBarsManager =
                realTimeBarsManager ?? throw new ArgumentNullException(nameof(realTimeBarsManager));
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

            var stream = realTimeBarsManager.Request(ContractHelper.GetStockContract(config.Symbol), WhatToShow.BID_ASK);
            await serializer.Save($"{config.Symbol}_realtime.csv", stream, token).ConfigureAwait(false);
            log.LogInformation("Realtime request completed");
        }
    }
}
