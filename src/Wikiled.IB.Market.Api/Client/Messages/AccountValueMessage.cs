namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AccountValueMessage 
    {
        public AccountValueMessage(string key, string value, string currency, string accountName)
        {
            Key = key;
            Value = value;
            Currency = currency;
            AccountName = accountName;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Currency { get; set; }

        public string AccountName { get; set; }
    }
}
