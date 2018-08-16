namespace Wikiled.IB.Market.Api.Client.Messages
{
    public abstract class MarketDataMessage
    {
        protected int requestId;

        protected int field;

        public MarketDataMessage(int requestId, int field)
        {
            RequestId = requestId;
            Field = field;
        }

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public int Field
        {
            get { return field; }
            set { field = value; }
        }
    }
}
