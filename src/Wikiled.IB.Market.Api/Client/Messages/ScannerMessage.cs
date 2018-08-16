namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ScannerMessage
    {
        public ScannerMessage(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            RequestId = reqId;
            Rank = rank;
            ContractDetails = contractDetails;
            Distance = distance;
            Benchmark = benchmark;
            Projection = projection;
            LegsStr = legsStr;
        }

        public int RequestId { get; set; }

        public int Rank { get; set; }

        public ContractDetails ContractDetails { get; set; }

        public string Distance { get; set; }

        public string Benchmark { get; set; }


        public string Projection { get; set; }


        public string LegsStr { get; set; }
    }
}
