using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;
using System.Threading;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public abstract class BaseDataManager<T> : IDisposable
        where T : IMessage
    {
        private static int currentTicker = 1;

        private readonly ReplaySubject<ErrorDescription> errors = new ReplaySubject<ErrorDescription>();

        private readonly int requestId;

        private Subject<T> stream;

        protected BaseDataManager(IBClient ibClient, ILoggerFactory logger)
        {
            IbClient = ibClient ?? throw new ArgumentNullException(nameof(ibClient));
            Logger = logger?.CreateLogger(GetType()) ?? throw new ArgumentNullException(nameof(logger));
            requestId = Interlocked.Increment(ref currentTicker);
            IbClient.Error += IbClientOnError;
        }

        protected ILogger Logger { get; }

        public int RequestId => RequestOffset + requestId;

        protected abstract int RequestOffset { get; }

        public IBClient IbClient { get; }

        public ErrorDescription LastErrors { get; private set; }

        public IObservable<ErrorDescription> Errors => errors;

        public virtual void Dispose()
        {
            IbClient.Error -= IbClientOnError;
            stream?.Dispose();
            errors.Dispose();
        }

        public virtual void Cancel()
        {
            stream?.OnCompleted();
        }

        protected virtual void OnMessage(T message)
        {
            Logger.LogDebug("OnMessage: {0}", message.RequestId);
            GetStream(message).OnNext(message);
        }

        protected Subject<T> Construct()
        {
            if (stream != null)
            {
                throw new InvalidOperationException("Stream is already active");
            }

            stream = new Subject<T>();
            return stream;
        }

        protected Subject<T> GetStream(T message)
        {
            if (message.RequestId == RequestId)
            {
                Logger.LogDebug("Received message: {0}", message.RequestId);
                return stream;
            }

            return null;
        }

        protected void OnCompleted(IMessage message)
        {
            if (message.RequestId == RequestId)
            {
                Logger.LogDebug("OnCompleted message: {0}", message.RequestId);
                stream.OnCompleted();
            }
        }

        private void IbClientOnError(ErrorDescription obj)
        {
            if (obj.Id == RequestId)
            {
                Logger.LogWarning("Error: {0}", obj);
                LastErrors = obj;
                errors.OnNext(obj);
            }
        }
    }
}