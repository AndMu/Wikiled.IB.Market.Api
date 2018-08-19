using System;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class ContractManager : BaseDataManager<ContractDetailsMessage>
    {
        public ContractManager(IBClient ibClient, ILoggerFactory logger)
            : base(ibClient, logger)
        {
            ibClient.ContractDetails += OnMessage;
            ibClient.ContractDetailsEnd += OnCompleted;
        }

        public override void Dispose()
        {
            IbClient.ContractDetails -= OnMessage;
            IbClient.ContractDetailsEnd -= OnCompleted;
            base.Dispose();
        }

        protected override int RequestOffset => MessageIdConstants.Contract;

        public IObservable<ContractDetailsMessage> Request(Contract contract)
        {
            var stream = Construct();
            IbClient.ClientSocket.ReqContractDetails(RequestId, contract);
            return stream;
        }
    }
}
