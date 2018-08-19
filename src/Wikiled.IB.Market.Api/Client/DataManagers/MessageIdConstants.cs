namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public static class MessageIdConstants
    {
        public const int TickNews = 90000000;

        public const int HistoricalNews = TickNews + 20000;

        public const int News = TickNews + 10000;

        public const int NewsProvider = 30000;

        public const int HistoricalData = 30000000;

        public const int RealTime = 40000000;

        public const int Contract = 60000000;
    }
}
