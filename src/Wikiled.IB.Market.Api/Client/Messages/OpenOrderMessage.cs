namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class OpenOrderMessage : OrderMessage
    {
        public OpenOrderMessage(int orderId, Contract contract, Order order, OrderState orderState)
        {
            OrderId = orderId;
            Contract = contract;
            Order = order;
            OrderState = orderState;
        }
        
        public Contract Contract { get; set; }

        public Order Order { get; set; }

        public OrderState OrderState { get; set; }
    }
}
