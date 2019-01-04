namespace Wikiled.IB.Market.Api.Client
{

    /// <summary>
    /// @class TickAttribBidAsk
    /// @brief Tick attributes that describes additional information for bid/ask price ticks
    /// @sa EWrapper::tickByTickBidAsk, EWrapper::historicalTicksBidAsk
    /// </summary>
    public class TickAttribBidAsk
    {
        /**
         * @brief Used with real time tick-by-tick. Indicates if bid is lower than day's lowest low. 
         */
        public bool BidPastLow { get; set; }

        /**
         * @brief Used with real time tick-by-tick. Indicates if ask is higher than day's highest ask. 
         */
        public bool AskPastHigh { get; set; }

        /**
         * @brief Returns string to display. 
         */
        public override string ToString()
        {
            return (BidPastLow ? "bidPastLow " : "") + (AskPastHigh ? "askPastHigh " : "");
        }
    }
}
