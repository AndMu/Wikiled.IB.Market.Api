using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.DataManagers;

namespace Wikiled.IB.Market.Api.Client
{
    public class IBClientWrapper : IDisposable
    {
        private readonly ILoggerFactory loggerFactory;

        private IBClient ibClient;

        private Thread processingThread;

        private readonly ILogger logger;

        public IBClientWrapper(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            logger = loggerFactory.CreateLogger<IBClientWrapper>();
        }

        public void Connect(string host, int port, int clientId)
        {
            logger.LogDebug("Connecting: {0}:{1} ({2})", host, port, clientId);
            if (ibClient != null)
            {
                throw new InvalidOperationException("Already connected");
            }

            EReaderMonitorSignal signal = new EReaderMonitorSignal();
            ibClient = new IBClient(signal);

            ibClient.ClientId = clientId;
            ibClient.ClientSocket.EConnect(host, port, clientId);
            var reader = new EReader(ibClient.ClientSocket, signal);
            reader.Start();

            processingThread = new Thread(() =>
                {
                    while (ibClient.ClientSocket.IsConnected)
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
        }

        public HistoricalDataManager GetHistoricalManager()
        {
            HistoricalDataManager historicalDataManager = new HistoricalDataManager(ibClient, loggerFactory);
            return historicalDataManager;;
        }

        public void Disconnect()
        {
            logger.LogDebug("Disconnect");
            ibClient?.ClientSocket?.EDisconnect();
            ibClient = null;
        }

        public void Dispose()
        {
            logger.LogDebug("Dispose");
            Disconnect();
        }
    }
}
