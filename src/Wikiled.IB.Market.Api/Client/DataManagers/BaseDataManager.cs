using System;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public abstract class BaseDataManager : IDisposable
    {
        protected BaseDataManager(IBClient ibClient)
        {
            IbClient = ibClient ?? throw new ArgumentNullException(nameof(ibClient));
        }

        public IBClient IbClient { get; }

        public abstract void Dispose();
    }
}
