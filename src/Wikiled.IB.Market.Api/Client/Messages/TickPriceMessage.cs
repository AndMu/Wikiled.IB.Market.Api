namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickPriceMessage : MarketDataMessage
    {
        public TickPriceMessage(int requestId, int field, double price, TickAttrib attribs)
            : base(requestId, field)
        {
            this.Price = price;
            this.Attribs = attribs;
        }

        public TickAttrib Attribs { get; set; }

        public double Price { get; set; }
    }
}
