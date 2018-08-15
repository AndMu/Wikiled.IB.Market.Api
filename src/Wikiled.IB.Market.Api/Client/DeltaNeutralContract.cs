namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Delta-Neutral Contract.
    */
    public class DeltaNeutralContract
    {
        /**
         * @brief The unique contract identifier specifying the security. Used for Delta-Neutral Combo contracts.
         */
        public int ConId { get; set; }

        /**
        * @brief The underlying stock or future delta. Used for Delta-Neutral Combo contracts.
        */
        public double Delta { get; set; }

        /**
        * @brief The price of the underlying. Used for Delta-Neutral Combo contracts.
        */
        public double Price { get; set; }
    }
}