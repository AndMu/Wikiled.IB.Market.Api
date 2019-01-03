using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Wikiled.IB.Market.Api.Console.Commands.Config
{
    public class NewsConfig : BaseConfig
    {
        [Required]
        public string From { get; }

        [Required]
        public string To { get; }
    }
}
