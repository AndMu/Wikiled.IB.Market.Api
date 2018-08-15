namespace Wikiled.IB.Market.Api.Client
{
    public class MessageValidator
    {
        public MessageValidator(int serverVersion)
        {
            ServerVersion = serverVersion;
        }

        public int ServerVersion { get; set; }
    }
}