namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ExecutionMessage
    {
        public ExecutionMessage(int reqId, Contract contract, Execution execution)
        {
            ReqId = reqId;
            Contract = contract;
            Execution = execution;
        }

        public Contract Contract { get; set; }

        public Execution Execution { get; set; }

        public int ReqId { get; set; }
    }
}
