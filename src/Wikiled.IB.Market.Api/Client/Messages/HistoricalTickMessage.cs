namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickMessage
    {
        public int ReqId { get; }

        public long Time { get; }

        public double Price { get; }

        public long Size { get; }

        public HistoricalTickMessage(int reqId, long time, double price, long size)
        {
            ReqId = reqId;
            Time = time;
            Price = price;
            Size = size;
        }
    }
}
