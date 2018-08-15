namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class PriceIncrement
     * @brief Class describing price increment
     * @sa EClient::reqMarketRule, EWrapper::marketRule
     */
    public class PriceIncrement
    {
        public PriceIncrement()
        {
        }

        public PriceIncrement(double lowEdge, double increment)
        {
            LowEdge = lowEdge;
            Increment = increment;
        }

        /**
         * @brief The low edge
         */
        public double LowEdge { get; set; }

        /**
         * @brief The increment
         */
        public double Increment { get; set; }
    }
}