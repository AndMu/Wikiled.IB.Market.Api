namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickLastMessage
    {
        public int ReqId { get; }

        public long Time { get; }

        public int Mask { get; }

        public double Price { get; }

        public long Size { get; }

        public string Exchange { get; }

        public string SpecialConditions { get; }

        public HistoricalTickLastMessage(int reqId, long time, int mask, double price, long size, string exchange, string specialConditions)
        {
            ReqId = reqId;
            Time = time;
            Mask = mask;
            Price = price;
            Size = size;
            Exchange = exchange;
            SpecialConditions = specialConditions;
        }
    }
}
