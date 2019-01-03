using NLog.Extensions.Logging;
using System.Threading.Tasks;
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
            var starter = new AutoStarter(ApplicationLogging.LoggerFactory, "IB Utility", args);
            starter.LoggerFactory.AddNLog();
            starter.RegisterCommand<HistoricCommand, HistoricConfig>("history");
            starter.RegisterCommand<RealtimeCommand, RealtimeConfig>("realtime");
            starter.RegisterCommand<NewsCommand, NewsConfig>("news");
            await new SyncExecutor(starter).Execute().ConfigureAwait(false);
            NLog.LogManager.Shutdown();
        }
    }
}
