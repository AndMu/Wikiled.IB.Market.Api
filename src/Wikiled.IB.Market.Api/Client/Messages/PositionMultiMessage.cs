namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class PositionMultiMessage 
    {
        public PositionMultiMessage(int reqId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            ReqId = reqId;
            Account = account;
            ModelCode = modelCode;
            Contract = contract;
            Position = pos;
            AverageCost = avgCost;
        }

        public int ReqId { get; set; }

        public string Account { get; set; }

        public string ModelCode { get; set; }

        public Contract Contract { get; set; }

        public double Position { get; set; }

        public double AverageCost { get; set; }
    }
}
