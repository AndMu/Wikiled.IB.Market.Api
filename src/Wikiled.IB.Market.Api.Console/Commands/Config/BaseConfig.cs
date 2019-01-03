using System.ComponentModel.DataAnnotations;
using Autofac;
using Newtonsoft.Json;
using System.IO;
using Wikiled.Console.Arguments;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Modules;

namespace Wikiled.IB.Market.Api.Console.Commands.Config
{
    public abstract class BaseConfig : ICommandConfig
    {
        [Required]
        public string Stock { get; set; }

        [Required]
        public string Out { get; set; }

        public virtual void Build(ContainerBuilder builder)
        {
            var json = File.ReadAllText("server.json");
            var config = JsonConvert.DeserializeObject<ServerConfig>(json);
            builder.RegisterModule(new MarketIBModule(config));
        }
    }
}
