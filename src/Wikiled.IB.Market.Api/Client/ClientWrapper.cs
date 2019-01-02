using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Wikiled.IB.Market.Api.Client
{
    public class ClientWrapper : IClientWrapper
    {
        private readonly IIBClient ibClient;

        private Thread processingThread;

        private readonly ILogger<ClientWrapper> logger;

        private bool connected;

        private readonly IEReaderSignal signal;

        private readonly ServerConfig config;

        public ClientWrapper(ILogger<ClientWrapper> logger, IIBClient ibClient, IEReaderSignal signal, ServerConfig config)
        {
            this.ibClient = ibClient ?? throw new ArgumentNullException(nameof(ibClient));
            this.signal = signal ?? throw new ArgumentNullException(nameof(signal));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public TimeZoneInfo TimeZone { get; set; } 

        public bool Connect()
        {
            logger.LogDebug("Connecting: {0}:{1} ({2})", config.Host, config.Port, config.ClientId);
            if (connected)
            {
                throw new InvalidOperationException("Already connected");
            }

            connected = true;
            ibClient.ClientId = config.ClientId;

            if (!ibClient.ClientSocket.EConnect(config.Host, config.Port, config.ClientId))
            {
                logger.LogError("Failed connection");
                return false;
            }

            var reader = new EReader(ibClient.ClientSocket, signal);
            reader.Start();

            processingThread = new Thread(() =>
                {
                    while (ibClient?.ClientSocket.IsConnected == true)
                    {
                        signal.WaitForSignal();
                        reader.ProcessMsgs();
                    }
                })
            {
                IsBackground = true
            };

            processingThread.Name = "IB Processing Thread";
            processingThread.Start();
            return true;
        }

        public void Disconnect()
        {
            logger.LogDebug("Disconnect");
            ibClient?.ClientSocket?.EDisconnect();
            processingThread.Join(1000);
        }

        public void Dispose()
        {
            logger.LogDebug("Dispose");
            Disconnect();
        }
    }
}
