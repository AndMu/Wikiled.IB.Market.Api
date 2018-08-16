namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistogramDataMessage
    {
        public int ReqId { get; }

        public HistogramEntry[] Data { get; }

        public HistogramDataMessage(int reqId, HistogramEntry[] data)
        {
            ReqId = reqId;
            Data = data;
        }
    }
}
