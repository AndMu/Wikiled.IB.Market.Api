﻿using System.Runtime.InteropServices;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class HistoricalTickBidAsk
     * @brief The historical tick's description. Used when requesting historical tick data with whatToShow = BID_ASK
     * @sa EClient, EWrapper
     */
    [ComVisible(true)]
    public class HistoricalTickBidAsk
    {
        public HistoricalTickBidAsk(long time, int mask, double priceBid, double priceAsk, long sizeBid, long sizeAsk)
        {
            Time = time;
            Mask = mask;
            PriceBid = priceBid;
            PriceAsk = priceAsk;
            SizeBid = sizeBid;
            SizeAsk = sizeAsk;
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
         * @brief The bid price of the historical tick
         */
        public double PriceBid { get; }

        /**
         * @brief The ask price of the historical tick 
         */
        public double PriceAsk { get; }

        /**
         * @brief The bid size of the historical tick 
         */
        public long SizeBid { [return: MarshalAs(UnmanagedType.I8)] get; }

        /**
         * @brief The ask size of the historical tick 
         */
        public long SizeAsk { [return: MarshalAs(UnmanagedType.I8)] get; }
    }
}