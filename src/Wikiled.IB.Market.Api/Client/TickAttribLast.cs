namespace Wikiled.IB.Market.Api.Client
{
    /// <summary>
    /// @class TickAttribLast
    /// @brief Tick attributes that describes additional information for last price ticks
    /// @sa EWrapper::tickByTickAllLast, EWrapper::historicalTicksLast
    /// </summary>
    public class TickAttribLast
    {
        /**
         * @brief Used with tick-by-tick last data or historical ticks last to indicate if a trade is halted
         */
        public bool PastLimit { get; set; }

        /**
         * @brief Used with tick-by-tick last data or historical ticks last to indicate if a trade is classified as 'unreportable' (odd lots, combos, derivative trades, etc)
        */
        public bool Unreported { get; set; }

        /**
         * @brief Returns string to display. 
         */
        public override string ToString()
        {
            return (PastLimit ? "pastLimit " : "") + (Unreported ? "unreported " : "");
        }
    }
}
