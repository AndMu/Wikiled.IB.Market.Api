using NLog.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Console.Commands;
using Wikiled.IB.Market.Api.Console.Commands.Config;

namespace Wikiled.IB.Market.Api.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            ApplicationLogging.LoggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog();
            });

            var starter = new AutoStarter(ApplicationLogging.LoggerFactory, "IB Utility", args);
            starter.RegisterCommand<OptionHistoryCommand, OptionsHistoricConfig>("options");
            starter.RegisterCommand<HistoricCommand, HistoricConfig>("history");
            starter.RegisterCommand<RealtimeCommand, RealtimeConfig>("realtime");
            starter.RegisterCommand<NewsCommand, NewsConfig>("news");
            await new SyncExecutor(starter).Execute().ConfigureAwait(false);
            NLog.LogManager.Shutdown();
        }
    }
}
