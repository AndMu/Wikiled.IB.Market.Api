using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.Serialization
{
    public interface ICsvSerializer
    {
        Task Save(string fileName, IObservable<IPriceData> data, CancellationToken token);
    }
}