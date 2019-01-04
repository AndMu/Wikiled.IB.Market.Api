namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickByTickAllLastMessage
    {
        public int ReqId { get; }

        public int TickType { get; }

        public long Time { get; }

        public double Price { get; }

        public long Size { get; }

        public TickAttribLast TickAttribLast { get; }

        public string Exchange { get; }

        public string SpecialConditions { get; }

        public TickByTickAllLastMessage(int reqId, int tickType, long time, double price, long size, TickAttribLast tickAttribLast, string exchange, string specialConditions)
        {
            ReqId = reqId;
            TickType = tickType;
            Time = time;
            Price = price;
            Size = size;
            TickAttribLast = tickAttribLast;
            Exchange = exchange;
            SpecialConditions = specialConditions;
        }
    }
}
