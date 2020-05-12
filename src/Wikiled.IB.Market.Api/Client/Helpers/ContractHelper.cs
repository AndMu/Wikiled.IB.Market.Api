using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Client.Helpers
{
    public class ContractHelper
    {
        public static Contract GetStockContract(string stock, ExchangeType exchange = ExchangeType.SMART, string currency = "USD")
        {
            var contract = new Contract
            {
                SecType = SecType.STK,
                Symbol = stock,
                Exchange = exchange,
                Currency = currency
            };

            return contract;
        }

        public static Contract GetOptionsContract(string option, double strike, string expiry, OptionType type, ExchangeType exchange = ExchangeType.SMART, string currency = "USD")
        {
            var contract = new Contract
                           {
                               SecType = SecType.OPT,
                               Symbol = option,
                               Exchange = exchange,
                               Currency = currency,
                               Right = type,
                               LastTradeDateOrContractMonth = expiry,
                               Strike = strike,
                               IncludeExpired = true
            };

            return contract;
        }
    }
}
