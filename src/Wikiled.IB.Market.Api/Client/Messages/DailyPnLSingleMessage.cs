namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class PnLSingleMessage
    {
        public int ReqId { get; }

        public int Pos { get; }

        public double DailyPnL { get; }

        public double Value { get; }

        public double UnrealizedPnL { get; }

        public double RealizedPnL { get; }

        public PnLSingleMessage(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
        {
            ReqId = reqId;
            Pos = pos;
            DailyPnL = dailyPnL;
            Value = value;
            UnrealizedPnL = unrealizedPnL;
            RealizedPnL = realizedPnL;
        }
    }
}
