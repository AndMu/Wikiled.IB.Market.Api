namespace Wikiled.IB.Market.Api.Client
{
    public enum TriggerMethod
    {
        Default = 0,
        DoubleBidAsk,
        Last,
        DoubleLast,
        BidAsk,
        LastOfBidAsk = 7,
        MidPoint
    }
}