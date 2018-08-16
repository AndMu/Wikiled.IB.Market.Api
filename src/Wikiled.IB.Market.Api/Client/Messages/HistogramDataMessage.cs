namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistogramDataMessage
    {
        public int ReqId { get; private set; }

        public HistogramEntry[] Data { get; private set; }

        public HistogramDataMessage(int reqId, HistogramEntry[] data)
        {
            ReqId = reqId;
            Data = data;
        }
    }
}
