namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AccountSummaryEndMessage 
    {
        public AccountSummaryEndMessage(int requestId)
        {
            RequestId = requestId;
        }

        public int RequestId { get; set; }
    }
}
