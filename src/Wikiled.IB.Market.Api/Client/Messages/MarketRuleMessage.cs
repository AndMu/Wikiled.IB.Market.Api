namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class MarketRuleMessage
    {
        public int MarketruleId { get; }

        public PriceIncrement[] PriceIncrements { get; }

        public MarketRuleMessage(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            MarketruleId = marketRuleId;
            PriceIncrements = priceIncrements;
        }
    }
}
