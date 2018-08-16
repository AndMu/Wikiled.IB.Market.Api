namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AccountDownloadEndMessage
    {
        public AccountDownloadEndMessage(string account)
        {
            Account = account;
        }

        public string Account { get; set; }
    }
}
