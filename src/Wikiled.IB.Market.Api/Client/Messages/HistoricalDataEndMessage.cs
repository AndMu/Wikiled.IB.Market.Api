namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class HistoricalDataEndMessage
    {
        public string StartDate { get; set; }

        public int RequestId { get; set; }

        public string EndDate { get; set; }

        public HistoricalDataEndMessage(int requestId, string startDate, string endDate)
        {
            RequestId = requestId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
