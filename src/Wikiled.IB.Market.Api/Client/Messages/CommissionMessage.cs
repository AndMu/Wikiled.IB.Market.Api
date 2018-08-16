namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class CommissionMessage
    {
        public CommissionMessage(CommissionReport commissionReport)
        {
            CommissionReport = commissionReport;
        }

        public CommissionReport CommissionReport { get; set; }
    }
}
