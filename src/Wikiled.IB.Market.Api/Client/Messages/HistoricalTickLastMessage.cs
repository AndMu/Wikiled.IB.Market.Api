namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickLastMessage
    {
        public HistoricalTickLastMessage(int reqId, long time, TickAttribLast tickAttribLast, double price, long size, string exchange, string specialConditions)
        {
            ReqId = reqId;
            Time = time;
            TickAttribLast = tickAttribLast;
            Price = price;
            Size = size;
            Exchange = exchange;
            SpecialConditions = specialConditions;
        }

        public string Exchange { get; }

        public TickAttribLast TickAttribLast { get; }

        public double Price { get; }

        public int ReqId { get; }

        public long Size { get; }

        public string SpecialConditions { get; }

        public long Time { get; }
    }
}
