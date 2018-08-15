namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class ContractDescription
     * @brief contract data and list of derivative security types
     * @sa Contract, EClient::reqMatchingSymbols, EWrapper::symbolSamples
     */
    public class ContractDescription
    {
        public ContractDescription()
        {
            Contract = new Contract();
        }

        public ContractDescription(Contract contract, string[] derivativeSecTypes)
        {
            Contract = contract;
            DerivativeSecTypes = derivativeSecTypes;
        }

        /**
         * @brief A contract data
         */
        public Contract Contract { get; set; }

        /**
         * @brief A list of derivative security types
         */
        public string[] DerivativeSecTypes { get; set; }

        public override string ToString()
        {
            return Contract + " derivativeSecTypes [" + string.Join(", ", DerivativeSecTypes) + "]";
        }
    }
}