namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AccountUpdateMultiMessage 
    {
        public AccountUpdateMultiMessage(int reqId, string account, string modelCode, string key, string value, string currency)
        {
            Account = account;
            ModelCode = modelCode;
            Key = key;
            Value = value;
            Currency = currency;
        }

        public int ReqId { get; set; }

        public string Account { get; set; }

        public string ModelCode { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Currency { get; set; }
    }
}
