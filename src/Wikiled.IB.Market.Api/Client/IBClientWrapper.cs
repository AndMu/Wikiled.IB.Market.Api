using Autofac;
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

        private ErrorManager errorManager;

        public IBClientWrapper(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            logger = loggerFactory.CreateLogger<IBClientWrapper>();
            Container = Construct();
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public IContainer Container { get; }

        public TimeZoneInfo TimeZone { get; set; }

        private IContainer Construct()
        {
            logger.LogDebug("Construct");
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(loggerFactory);
            builder.Register(cnt => ibClient);
            builder.RegisterType<HistoricalDataManager>();
            builder.RegisterType<ContractManager>();
            builder.RegisterType<HistoricalNewsManager>();
            builder.RegisterType<NewsManager>();
            builder.RegisterType<NewsProviderManager>();
            builder.RegisterType<RealTimeBarsManager>();
            builder.RegisterType<TickNewsManager>();
            builder.RegisterType<ErrorManager>();
            return builder.Build();
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

            errorManager = Container.Resolve<ErrorManager>();
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

        public T GetManager<T>() where T : IDataManager
        {
            return Container.Resolve<T>();
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
            errorManager.Dispose();
        }
    }
}
