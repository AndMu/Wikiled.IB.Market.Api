namespace Wikiled.IB.Market.Api.Client.Messages
{
    public interface IPriceData
    {
        string Time { get; }

        double Open { get; }

        double High { get; }

        double Low { get; }

        double Close { get; }

        long Volume { get; }
    }
}
