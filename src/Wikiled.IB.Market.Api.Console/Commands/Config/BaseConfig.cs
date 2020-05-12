using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Modules;
using Wikiled.Common.Utilities.Modules;

namespace Wikiled.IB.Market.Api.Console.Commands.Config
{
    public abstract class BaseConfig : ICommandConfig
    {
        [Required]
        public string Symbol { get; set; }

        public void Build(IServiceCollection services)
        {
            var json = File.ReadAllText("server.json");
            var config = JsonConvert.DeserializeObject<ServerConfig>(json);
            services.RegisterModule(new MarketIBModule(config));
        }
    }
}
