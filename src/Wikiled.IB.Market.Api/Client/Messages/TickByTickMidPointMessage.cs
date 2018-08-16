namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickByTickMidPointMessage
    {
        public int ReqId { get; private set; }

        public long Time { get; private set; }

        public double MidPoint { get; private set; }

        public TickByTickMidPointMessage(int reqId, long time, double midPoint)
        {
            ReqId = reqId;
            Time = time;
            MidPoint = midPoint;
        }
    }
}
