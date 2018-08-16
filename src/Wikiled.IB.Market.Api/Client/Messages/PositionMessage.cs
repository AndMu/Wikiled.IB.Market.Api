namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class PositionMessage 
    {
        public PositionMessage(string account, Contract contract, double pos, double avgCost)
        {
            Account = account;
            Contract = contract;
            Position = pos;
            AverageCost = avgCost;
        }

        public string Account { get; set; }

        public Contract Contract { get; set; }

        public double Position { get; set; }

        public double AverageCost { get; set; }
    }
}
