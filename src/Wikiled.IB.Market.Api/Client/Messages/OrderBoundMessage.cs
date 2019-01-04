namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class OrderBoundMessage
    {
        public OrderBoundMessage(long orderId, int apiClientId, int apiOrderId)
        {
            OrderId = orderId;
            ApiClientId = apiClientId;
            ApiOrderId = apiOrderId;
        }

        public long OrderId { get; }

        public int ApiClientId { get; }

        public int ApiOrderId { get; }
    }
}
