namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ScannerEndMessage
    {
        public ScannerEndMessage(int requestId)
        {
             RequestId = requestId;
        }

        public int RequestId { get; set; }
    }
}
