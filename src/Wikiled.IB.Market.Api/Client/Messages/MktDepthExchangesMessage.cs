namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class MktDepthExchangesMessage
    {
        public DepthMktDataDescription[] Descriptions { get; }

        public MktDepthExchangesMessage(DepthMktDataDescription[] descriptions)
        {
            Descriptions = descriptions;
        }
    }
}
