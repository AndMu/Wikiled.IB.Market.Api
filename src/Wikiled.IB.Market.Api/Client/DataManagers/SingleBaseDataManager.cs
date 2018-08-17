using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public abstract class SingleBaseDataManager<T> : BaseDataManager<T>
        where T : IMessage
    {
        protected SingleBaseDataManager(IBClient ibClient, ILoggerFactory logger)
            : base(ibClient, logger)
        {
        }

        protected override void OnMessage(T message)
        {
            base.OnMessage(message);
            OnCompleted(message);
        }
    }
}
