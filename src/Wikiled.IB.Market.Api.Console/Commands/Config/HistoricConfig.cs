using Autofac;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Modules;

namespace Wikiled.IB.Market.Api.Console.Commands.Config
{
    public class HistoricConfig : ICommandConfig
    {
        [Required]
        public string Stock { get; set; }

        [Required]
        public string Out { get; set; }

        public void Build(ContainerBuilder builder)
        {
            var json = File.ReadAllText("server.json");
            var config = JsonConvert.DeserializeObject<ServerConfig>(json);
            builder.RegisterModule(new MarketIBModule(config));
        }
    }
}
