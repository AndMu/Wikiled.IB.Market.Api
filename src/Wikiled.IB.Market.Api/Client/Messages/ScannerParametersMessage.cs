namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ScannerParametersMessage
    {
        public ScannerParametersMessage(string data)
        {
            XmlData = data;
        }

        public string XmlData { get; set; }
    }
}
