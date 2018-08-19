namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ContractDetailsMessage : IMessage
    {
        public ContractDetailsMessage(int requestId, ContractDetails contractDetails)
        {
            RequestId = requestId;
            ContractDetails = contractDetails;
        }

        public ContractDetails ContractDetails { get; }

        public int RequestId { get; }
    }
}
