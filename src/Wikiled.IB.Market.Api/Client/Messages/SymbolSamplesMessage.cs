namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class SymbolSamplesMessage
    {
        public int ReqId { get; }

        public ContractDescription[] ContractDescriptions { get; }

        public SymbolSamplesMessage(int reqId, ContractDescription[] contractDescriptions)
        {
            ReqId = reqId;
            ContractDescriptions = contractDescriptions;
        }
    }
}
