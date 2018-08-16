namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class FundamentalsMessage
    {
        public FundamentalsMessage(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}
