namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class UpdateAccountTimeMessage
    {
        public UpdateAccountTimeMessage(string timestamp)
        {
            Timestamp = timestamp;
        }

        public string Timestamp { get; set; }
    }
}
