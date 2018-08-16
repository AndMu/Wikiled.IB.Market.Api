namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class SymbolSamplesMessage
    {
        public int ReqId { get; private set; }

        public ContractDescription[] ContractDescriptions { get; private set; }

        public SymbolSamplesMessage(int reqId, ContractDescription[] contractDescriptions)
        {
            this.ReqId = reqId;
            this.ContractDescriptions = contractDescriptions;
        }
    }
}
