using System.Collections.Generic;

namespace Wikiled.IB.Market.Api.Client
{
    public class Liquidity
    {
        /**
         * @brief The enum of available liquidity flag types. 
         * 0 = Unknown, 1 = Added liquidity, 2 = Removed liquidity, 3 = Liquidity routed out
         */

        private static readonly Dictionary<int, string> Values = new Dictionary<int, string>
        {
            { 0, "None" },
            { 1, "Added Liquidity" },
            { 2, "Removed Liquidity" },
            { 3, "Liquidity Routed Out" }
        };

        public Liquidity(int p)
        {
            Value = Values.ContainsKey(p) ? p : 0;
        }

        /**
         * @brief The value of the liquidity type.
         */
        public int Value { get; set; }

        public override string ToString()
        {
            return Values[Value];
        }
    }
}