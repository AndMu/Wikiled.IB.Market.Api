namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalDataMessage : IMessage
    {
        public HistoricalDataMessage(int reqId, Bar bar)
        {
            RequestId = reqId;
            Date = bar.Time;
            Open = bar.Open;
            High = bar.High;
            Low = bar.Low;
            Close = bar.Close;
            Volume = bar.Volume;
            Count = bar.Count;
            Wap = bar.Wap;
        }

        public string Date { get; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public long Volume { get; set; }

        public int Count { get; set; }

        public double Wap { get; set; }

        public int RequestId { get; }
    }
}