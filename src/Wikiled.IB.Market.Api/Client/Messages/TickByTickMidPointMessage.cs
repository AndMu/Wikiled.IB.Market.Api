namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickByTickMidPointMessage
    {
        public int ReqId { get; }

        public long Time { get; }

        public double MidPoint { get; }

        public TickByTickMidPointMessage(int reqId, long time, double midPoint)
        {
            ReqId = reqId;
            Time = time;
            MidPoint = midPoint;
        }
    }
}
