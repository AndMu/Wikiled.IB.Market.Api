using System;
using Autofac;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;

namespace Wikiled.IB.Market.Api.Modules
{
    public class MarketIBModule : Module
    {
        private readonly ServerConfig config;

        public MarketIBModule(ServerConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(config);
            builder.RegisterType<IBClient>().As<IIBClient>().AsSelf().SingleInstance();
            builder.RegisterType<ClientWrapper>().As<IClientWrapper>().SingleInstance();
            builder.RegisterType<EReaderMonitorSignal>().As<IEReaderSignal>().AsSelf().SingleInstance();
            builder.RegisterType<HistoricalDataManager>();
            builder.RegisterType<ContractManager>();
            builder.RegisterType<HistoricalNewsManager>();
            builder.RegisterType<NewsManager>();
            builder.RegisterType<NewsProviderManager>();
            builder.RegisterType<RealTimeBarsManager>();
            builder.RegisterType<TickNewsManager>();
            builder.RegisterType<ErrorManager>();
            base.Load(builder);
        }
    }
}
