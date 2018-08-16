namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class MarketRuleMessage
    {
        public int MarketruleId { get; private set; }

        public PriceIncrement[] PriceIncrements { get; private set; }

        public MarketRuleMessage(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            this.MarketruleId = marketRuleId;
            this.PriceIncrements = priceIncrements;
        }
    }
}
