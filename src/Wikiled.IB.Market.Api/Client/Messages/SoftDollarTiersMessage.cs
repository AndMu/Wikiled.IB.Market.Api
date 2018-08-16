namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class SoftDollarTiersMessage
    {
        public int ReqId { get; }
        public SoftDollarTier[] Tiers { get; }

        public SoftDollarTiersMessage(int reqId, SoftDollarTier[] tiers)
        {
            ReqId = reqId;
            Tiers = tiers;
        }
    }
}
