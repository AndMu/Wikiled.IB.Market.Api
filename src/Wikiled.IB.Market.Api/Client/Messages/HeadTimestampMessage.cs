namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HeadTimestampMessage
    {
        public int ReqId { get; }
        public string HeadTimestamp { get; }

        public HeadTimestampMessage(int reqId, string headTimestamp)
        {
            ReqId = reqId;
            HeadTimestamp = headTimestamp;
        }
    }
}
