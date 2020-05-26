using System;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Common.Utilities.Modules;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;
using Wikiled.IB.Market.Api.Client.Serialization;

namespace Wikiled.IB.Market.Api.Modules
{
    public class MarketIBModule : IModule
    {
        private readonly ServerConfig config;

        public MarketIBModule(ServerConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(config);
            services.AddSingleton<IBClient>().As<IIBClient, IBClient>();
            services.AddSingleton<IClientWrapper, ClientWrapper>();
            services.AddSingleton<IEReaderSignal, EReaderMonitorSignal>();
            services.AddTransient<ICsvSerializer, CsvSerializer>();
            services.AddTransientWithFactory<HistoricalDataManager, HistoricalDataManager>();
            services.AddTransient<ContractManager>();
            services.AddTransient<HistoricalNewsManager>();

            services.AddTransient<NewsManager>();
            services.AddTransient<NewsProviderManager>();
            services.AddTransient<RealTimeBarsManager>();
            services.AddTransient<TickNewsManager>();
            services.AddTransient<ErrorManager>();

            return services;
        }
    }
}
