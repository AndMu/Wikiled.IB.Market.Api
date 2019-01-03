using System;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalDataMessage : IMessage, IPriceData
    {
        private readonly Bar bar;

        public HistoricalDataMessage(int requestId, Bar bar)
        {
            this.bar = bar ?? throw new ArgumentNullException(nameof(bar));
            RequestId = requestId;
        }

        public string Time => bar.Time;

        public double Open => bar.Open;

        public double High => bar.High;

        public double Low => bar.Low;

        public double Close => bar.Close;

        public long Volume => bar.Volume;

        public int Count => bar.Count;

        public double Wap => bar.Wap;

        public int RequestId { get; }

        public override string ToString()
        {
            return $"Price [{Time}] {Open}:{Close} {High}:{Low} {Volume} {Count} {Wap}";
        }
    }
}