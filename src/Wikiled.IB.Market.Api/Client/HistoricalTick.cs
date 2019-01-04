using System.Runtime.InteropServices;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class HistoricalTick
     * @brief The historical tick's description. Used when requesting historical tick data with whatToShow = MIDPOINT
     * @sa EClient, EWrapper
     */
    [ComVisible(true)]
    public class HistoricalTick
    {
        public HistoricalTick()
        {
        }

        public HistoricalTick(long time, double price, long size)
        {
            Time = time;
            Price = price;
            Size = size;
        }

        /**
         * @brief The UNIX timestamp of the historical tick 
         */
        public long Time { [return: MarshalAs(UnmanagedType.I8)] get; }

        /**
         * @brief The historical tick price
         */
        public double Price { get; }

        /**
         * @brief The historical tick size
         */
        public long Size { [return: MarshalAs(UnmanagedType.I8)] get; }
    }
}