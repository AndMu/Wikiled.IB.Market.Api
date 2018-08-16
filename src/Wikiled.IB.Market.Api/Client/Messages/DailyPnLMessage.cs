namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class PnLMessage
    {
        public int ReqId { get; }

        public double DailyPnL { get; }

        public double UnrealizedPnL { get; }

        public double RealizedPnL { get; }

        public PnLMessage(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            ReqId = reqId;
            DailyPnL = dailyPnL;
            UnrealizedPnL = unrealizedPnL;
            RealizedPnL = realizedPnL;
        }
    }
}
