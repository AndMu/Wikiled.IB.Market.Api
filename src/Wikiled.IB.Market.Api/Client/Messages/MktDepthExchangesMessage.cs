namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class MktDepthExchangesMessage
    {
        public DepthMktDataDescription[] Descriptions { get; private set; }

        public MktDepthExchangesMessage(DepthMktDataDescription[] descriptions)
        {
            this.Descriptions = descriptions;
        }
    }
}
