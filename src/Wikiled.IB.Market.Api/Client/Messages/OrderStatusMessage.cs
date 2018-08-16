namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class OrderStatusMessage : OrderMessage
    {
        public string Status { get; }

        public double Filled { get; }

        public double Remaining { get; }

        public double AvgFillPrice { get; }

        public int PermId { get; }

        public int ParentId { get; }

        public double LastFillPrice { get; }

        public int ClientId { get; }

        public string WhyHeld { get; }

        public double MktCapPrice { get; private set; }

        public OrderStatusMessage(int orderId, string status, double filled, double remaining, double avgFillPrice,
           int permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            OrderId = orderId;
            Status = status;
            Filled = filled;
            Remaining = remaining;
            AvgFillPrice = avgFillPrice;
            PermId = permId;
            ParentId = parentId;
            LastFillPrice = lastFillPrice;
            ClientId = clientId;
            WhyHeld = whyHeld;
        }       
    }
}
