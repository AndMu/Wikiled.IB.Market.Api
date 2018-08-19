using System;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public abstract class BaseDataManager<T> : IDataManager where T : IMessage
    {
        private static int currentTicker = 1;

        private readonly ReplaySubject<IErrorDescription> errors = new ReplaySubject<IErrorDescription>();

        private readonly int requestId;

        private Subject<T> stream;

        protected BaseDataManager(IBClient ibClient, ILoggerFactory logger)
        {
            IbClient = ibClient ?? throw new ArgumentNullException(nameof(ibClient));
            Logger = logger?.CreateLogger(GetType()) ?? throw new ArgumentNullException(nameof(logger));
            requestId = Interlocked.Increment(ref currentTicker);
            IbClient.Error += IbClientOnError;
        }

        public virtual int RequestId => RequestOffset + requestId;

        public IObservable<IErrorDescription> Errors => errors;

        public IBClient IbClient { get; }

        public IErrorDescription LastErrors { get; private set; }

        protected abstract int RequestOffset { get; }

        protected ILogger Logger { get; }

        public virtual void Cancel()
        {
            stream?.OnCompleted();
        }

        public virtual void Dispose()
        {
            IbClient.Error -= IbClientOnError;
            stream?.Dispose();
            errors.Dispose();
        }

        protected virtual void OnMessage(T message)
        {
            GetStream(message)?.OnNext(message);
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

        protected void OnCompleted(IMessage message)
        {
            OnCompleted(message.RequestId);
        }

        protected void OnCompleted(int id)
        {
            if (id == RequestId)
            {
                Logger.LogDebug("OnCompleted message: {0}", id);
                stream.OnCompleted();
            }
        }

        private Subject<T> GetStream(T message)
        {
            if (message.RequestId == RequestId)
            {
                Logger.LogTrace("Received message: {0}", message.RequestId);
                return stream;
            }

            return null;
        }

        private void IbClientOnError(IErrorDescription obj)
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
