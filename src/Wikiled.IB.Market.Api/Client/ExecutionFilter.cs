namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class ExecutionFilter
     * @brief when requesting executions, a filter can be specified to receive only a subset of them
     * @sa Contract, Execution, CommissionReport
     */
    public class ExecutionFilter
    {
        public ExecutionFilter()
        {
            ClientId = 0;
        }

        public ExecutionFilter(int clientId,
                               string acctCode,
                               string time,
                               string symbol,
                               string secType,
                               string exchange,
                               string side)
        {
            ClientId = clientId;
            AcctCode = acctCode;
            Time = time;
            Symbol = symbol;
            SecType = secType;
            Exchange = exchange;
            Side = side;
        }

        /**
         * @brief The API client which placed the order
         */
        public int ClientId { get; set; }

        /**
        * @brief The account to which the order was allocated to
        */
        public string AcctCode { get; set; }

        /**
         * @brief Time from which the executions will be brough yyyymmdd hh:mm:ss
         * Only those executions reported after the specified time will be returned.
         */
        public string Time { get; set; }

        /**
        * @brief The instrument's symbol
        */
        public string Symbol { get; set; }

        /**
         * @brief The Contract's security's type (i.e. STK, OPT...)
         */
        public string SecType { get; set; }

        /**
         * @brief The exchange at which the execution was produced
         */
        public string Exchange { get; set; }

        /**
        * @brief The Contract's side (Put or Call).
        */
        public string Side { get; set; }

        public override bool Equals(object other)
        {
            var lBRetVal = false;

            if (other == null)
            {
                lBRetVal = false;
            }
            else if (this == other)
            {
                lBRetVal = true;
            }
            else
            {
                var lTheOther = (ExecutionFilter)other;
                lBRetVal = ClientId == lTheOther.ClientId &&
                    string.Compare(AcctCode, lTheOther.AcctCode, true) == 0 &&
                    string.Compare(Time, lTheOther.Time, true) == 0 &&
                    string.Compare(Symbol, lTheOther.Symbol, true) == 0 &&
                    string.Compare(SecType, lTheOther.SecType, true) == 0 &&
                    string.Compare(Exchange, lTheOther.Exchange, true) == 0 &&
                    string.Compare(Side, lTheOther.Side, true) == 0;
            }

            return lBRetVal;
        }
    }
}