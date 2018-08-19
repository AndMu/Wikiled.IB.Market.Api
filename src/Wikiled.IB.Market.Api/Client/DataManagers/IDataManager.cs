using System;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public interface IDataManager : IDisposable
    {
        int RequestId { get; }

        IObservable<IErrorDescription> Errors { get; }

        IBClient IbClient { get; }

        IErrorDescription LastErrors { get; }

        void Cancel();
    }
}