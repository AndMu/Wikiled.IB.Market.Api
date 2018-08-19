namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalDataMessage : IMessage
    {
        public HistoricalDataMessage(int requestId, Bar bar)
        {
            RequestId = requestId;
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

        public double Open { get; }

        public double High { get; }

        public double Low { get; }

        public double Close { get; }

        public long Volume { get; }

        public int Count { get; }

        public double Wap { get; }

        public int RequestId { get; }

        public override string ToString()
        {
            return $"Price [{Date}] {Open}:{Close} {High}:{Low} {Volume} {Count} {Wap}";
        }
    }
}