namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickOptionMessage : MarketDataMessage
    {
        public TickOptionMessage(int requestId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
            : base(requestId, field)
        {
            ImpliedVolatility = impliedVolatility;
            Delta = delta;
            OptPrice = optPrice;
            PvDividend = pvDividend;
            Gamma = gamma;
            Vega = vega;
            Theta = theta;
            UndPrice = undPrice;
        }

        public double ImpliedVolatility { get; set; }

        public double Delta { get; set; }

        public double OptPrice { get; set; }

        public double PvDividend { get; set; }

        public double Gamma { get; set; }

        public double Vega { get; set; }

        public double Theta { get; set; }

        public double UndPrice { get; set; }
    }
}
