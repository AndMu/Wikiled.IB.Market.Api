using System;
using System.ComponentModel.DataAnnotations;
using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Console.Commands.Config
{
    public class OptionsHistoricConfig : BaseConfig
    {
        [Required]
        public string Expiry { get; set; }

        [Required]
        public OptionType Type { get; set; }

        [Required]
        public double Strike { get; set; }
    }
}
