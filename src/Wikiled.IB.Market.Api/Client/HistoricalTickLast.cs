using System.Runtime.InteropServices;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class HistoricalTickLast
     * @brief The historical tick's description. Used when requesting historical tick data with whatToShow = TRADES
     * @sa EClient, EWrapper
     */
    [ComVisible(true)]
    public class HistoricalTickLast
    {
        public HistoricalTickLast(long time,
                                  int mask,
                                  double price,
                                  long size,
                                  string exchange,
                                  string specialConditions)
        {
            Time = time;
            Mask = mask;
            Price = price;
            Size = size;
            Exchange = exchange;
            SpecialConditions = specialConditions;
        }

        /**
         * @brief The UNIX timestamp of the historical tick 
         */
        public long Time { [return: MarshalAs(UnmanagedType.I8)] get; }

        /**
         * @brief Mask
         */
        public int Mask { get; }

        /**
         * @brief The last price of the historical tick 
         */
        public double Price { get; }

        /**
         * @brief The last size of the historical tick 
         */
        public long Size { [return: MarshalAs(UnmanagedType.I8)] get; }

        /**
         * @brief The source exchange of the historical tick 
         */
        public string Exchange { get; }

        /**
         * @brief The conditions of the historical tick. Refer to Trade Conditions page for more details: https://www.interactivebrokers.com/en/index.php?f=7235
         */
        public string SpecialConditions { get; }
    }
}