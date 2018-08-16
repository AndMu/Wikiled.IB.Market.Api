namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalTickBidAskMessage
    {
        public int ReqId { get; set; }

        public long Time { get; set; }

        public int Mask { get; set; }

        public double PriceBid { get; set; }

        public double PriceAsk { get; set; }

        public long SizeBid { get; set; }

        public long SizeAsk { get; set; }

        public HistoricalTickBidAskMessage(int reqId, long time, int mask, double priceBid, double priceAsk, long sizeBid, long sizeAsk)
        {
            ReqId = reqId;
            Time = time;
            Mask = mask;
            PriceBid = priceBid;
            PriceAsk = priceAsk;
            SizeBid = sizeBid;
            SizeAsk = sizeAsk;
        }
    }
}
