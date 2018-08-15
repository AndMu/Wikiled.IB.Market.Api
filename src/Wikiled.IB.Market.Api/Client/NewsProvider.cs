namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class NewsProvider
     * @brief Class describing news provider
     * @sa EClient::reqNewsProviders, EWrapper::newsProviders
     */
    public class NewsProvider
    {
        public NewsProvider()
        {
        }

        public NewsProvider(string providerCode, string providerName)
        {
            ProviderCode = providerCode;
            ProviderName = providerName;
        }

        /**
         * @brief The API news provider code
         */
        public string ProviderCode { get; set; }

        /**
         * @brief The API news provider name
         */
        public string ProviderName { get; set; }
    }
}