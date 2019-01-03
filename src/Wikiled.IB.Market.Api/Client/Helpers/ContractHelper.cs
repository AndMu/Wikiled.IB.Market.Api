namespace Wikiled.IB.Market.Api.Client.Helpers
{
    public class ContractHelper
    {
        public static Contract GetContract(string stock)
        {
            var contract = new Contract
            {
                SecType = SecType.STK,
                Symbol = stock,
                Exchange = ExchangeType.ISLAND,
                Currency = "USD"
            };

            return contract;
        }
    }
}
