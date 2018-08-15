namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class FamilyCode
     * @brief Class describing family code
     * @sa EClient::reqFamilyCodes, EWrapper::familyCodes
     */
    public class FamilyCode
    {
        public FamilyCode()
        {
        }

        public FamilyCode(string accountId, string familyCodeStr)
        {
            AccountId = accountId;
            FamilyCodeStr = familyCodeStr;
        }

        /**
         * @brief The API account id
         */
        public string AccountId { get; set; }

        /**
         * @brief The API family code
         */
        public string FamilyCodeStr { get; set; }
    }
}