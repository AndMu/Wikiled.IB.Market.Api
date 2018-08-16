namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ContractDetailsMessage
    {
        public ContractDetailsMessage(int requestId, ContractDetails contractDetails)
        {
            RequestId = requestId;
            ContractDetails = contractDetails;
        }

        public ContractDetails ContractDetails { get; set; }

        public int RequestId { get; set; }
    }
}
