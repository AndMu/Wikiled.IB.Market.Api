using System.Collections.Generic;

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

        public ExecutionFilter(int clientId, string acctCode, string time, string symbol, string secType, string exchange, string side)
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
        * @brief Time from which the executions will be returned yyyymmdd hh:mm:ss
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
       * @brief The Contract's side (BUY or SELL)
       */
        public string Side { get; set; }

        protected bool Equals(ExecutionFilter other)
        {
            return ClientId == other.ClientId &&
                   AcctCode == other.AcctCode &&
                   Time == other.Time &&
                   Symbol == other.Symbol &&
                   SecType == other.SecType &&
                   Exchange == other.Exchange &&
                   Side == other.Side;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExecutionFilter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ClientId;
                hashCode = (hashCode * 397) ^ (AcctCode != null ? AcctCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Time != null ? Time.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Symbol != null ? Symbol.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SecType != null ? SecType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Exchange != null ? Exchange.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Side != null ? Side.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}