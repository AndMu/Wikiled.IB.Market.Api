namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickSizeMessage : MarketDataMessage
    {
        public TickSizeMessage(int requestId, int field, int size) : base(requestId, field)
        {
            Size = size;
        }

        public int Size { get; set; }
    }
}
