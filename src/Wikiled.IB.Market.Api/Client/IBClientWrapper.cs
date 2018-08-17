using Microsoft.Extensions.Logging;
using System;
using System.Threading;
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
            ibClient = new IBClient(signal)
            {
                ClientId = clientId
            };
            ibClient.ClientSocket.EConnect(host, port, clientId);
            EReader reader = new EReader(ibClient.ClientSocket, signal);
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
        }

        public HistoricalDataManager GetHistorical()
        {
            return new HistoricalDataManager(ibClient, loggerFactory);
        }

        public RealTimeBarsManager GetRealtime()
        {
            return new RealTimeBarsManager(ibClient, loggerFactory);
        }

        public HistoricalNewsManager GetHistoricalNews()
        {
            return new HistoricalNewsManager(ibClient, loggerFactory);
        }

        public TickNewsManager GetTickNews()
        {
            return new TickNewsManager(ibClient, loggerFactory);
        }

        public NewsManager GetNews()
        {
            return new NewsManager(ibClient, loggerFactory);
        }

        public NewsProviderManager GetNewsProvider()
        {
            return new NewsProviderManager(ibClient, loggerFactory);
        }

        public void Disconnect()
        {
            logger.LogDebug("Disconnect");
            ibClient?.ClientSocket?.EDisconnect();
            processingThread.Join(1000);
            ibClient = null;
        }

        public void Dispose()
        {
            logger.LogDebug("Dispose");
            Disconnect();
        }
    }
}
