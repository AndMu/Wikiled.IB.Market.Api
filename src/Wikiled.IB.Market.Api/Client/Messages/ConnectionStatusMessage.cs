namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ConnectionStatusMessage
    {
        public bool IsConnected { get; }

        public ConnectionStatusMessage(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}
