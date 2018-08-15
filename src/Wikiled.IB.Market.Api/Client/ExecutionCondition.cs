using System.IO;

namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @class ExecutionCondition
    * @brief This class represents a condition requiring a specific execution event to be fulfilled.
    * Orders can be activated or canceled if a set of given conditions is met. An ExecutionCondition is met whenever a trade occurs on a certain product at the given exchange.
    */
    public class ExecutionCondition : OrderCondition
    {
        private const string Header = "trade occurs for ",
            SymbolSuffix = " symbol on ",
            ExchangeSuffix = " exchange for ",
            SecTypeSuffix = " security type";

        /**
        * @brief Exchange where the symbol needs to be traded.
        */
        public string Exchange { get; set; }

        /**
        * @brief Kind of instrument being monitored.
        */
        public string SecType { get; set; }

        /**
        * @brief Instrument's symbol
        */
        public string Symbol { get; set; }

        public override string ToString()
        {
            return Header + Symbol + SymbolSuffix + Exchange + ExchangeSuffix + SecType + SecTypeSuffix;
        }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(Header))
            {
                return false;
            }

            try
            {
                var parser = new StringSuffixParser(cond.Replace(Header, ""));

                Symbol = parser.GetNextSuffixedValue(SymbolSuffix);
                Exchange = parser.GetNextSuffixedValue(ExchangeSuffix);
                SecType = parser.GetNextSuffixedValue(SecTypeSuffix);

                if (!string.IsNullOrWhiteSpace(parser.Rest))
                {
                    return base.TryParse(parser.Rest);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override void Deserialize(IDecoder inStream)
        {
            base.Deserialize(inStream);

            SecType = inStream.ReadString();
            Exchange = inStream.ReadString();
            Symbol = inStream.ReadString();
        }

        public override void Serialize(BinaryWriter outStream)
        {
            base.Serialize(outStream);

            outStream.AddParameter(SecType);
            outStream.AddParameter(Exchange);
            outStream.AddParameter(Symbol);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExecutionCondition;

            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) &&
                Exchange.Equals(other.Exchange) &&
                SecType.Equals(other.SecType) &&
                Symbol.Equals(other.Symbol);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Exchange.GetHashCode() + SecType.GetHashCode() + Symbol.GetHashCode();
        }
    }
}