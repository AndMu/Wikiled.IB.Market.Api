namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalDataEndMessage : IMessage
    {
        public HistoricalDataEndMessage(int requestId, string startDate, string endDate)
        {
            RequestId = requestId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int RequestId { get; }
    }
}