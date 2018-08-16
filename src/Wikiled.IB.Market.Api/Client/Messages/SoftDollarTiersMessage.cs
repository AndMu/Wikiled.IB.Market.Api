namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class SoftDollarTiersMessage
    {
        public int ReqId { get; private set; }
        public SoftDollarTier[] Tiers { get; private set; }

        public SoftDollarTiersMessage(int reqId, SoftDollarTier[] tiers)
        {
            this.ReqId = reqId;
            this.Tiers = tiers;
        }
    }
}
