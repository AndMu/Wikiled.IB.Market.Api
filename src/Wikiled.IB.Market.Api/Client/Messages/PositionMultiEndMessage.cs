namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class PositionMultiEndMessage 
    {
        public PositionMultiEndMessage(int reqId)
        {
            ReqId = reqId;
        }

        public int ReqId { get; set; }
    }
}
