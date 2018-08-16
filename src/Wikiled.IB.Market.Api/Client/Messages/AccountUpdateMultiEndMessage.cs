namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AccountUpdateMultiEndMessage 
    {
        public AccountUpdateMultiEndMessage(int reqId)
        {
            ReqId = ReqId;
        }

        public int ReqId { get; set; }
    }
}
