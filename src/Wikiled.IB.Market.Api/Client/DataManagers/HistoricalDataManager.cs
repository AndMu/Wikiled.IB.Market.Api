using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class HistoricalDataManager : BaseDataManager
    {
        public const int HistoricalIdBase = 30000000;

        private static int CurrentTicker = 1;

        private readonly string fullDatePattern = "yyyyMMdd  HH:mm:ss";

        private readonly string yearMonthDayPattern = "yyyyMMdd";

        private readonly ConcurrentDictionary<int, Subject<HistoricalDataMessage>> historicalDataStreams = new ConcurrentDictionary<int, Subject<HistoricalDataMessage>>();

        private readonly ILogger logger;

        public HistoricalDataManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<HistoricalDataManager>();
            ibClient.HistoricalData += UpdateUi;
            ibClient.HistoricalDataUpdate += UpdateUi;
            ibClient.HistoricalDataEnd += OnCompleted;
        }

        public IObservable<HistoricalDataMessage> Request(Contract contract, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRth, int dateFormat, bool keepUpToDate)
        {
            var historicalData = new Subject<HistoricalDataMessage>();
            var id = Interlocked.Increment(ref CurrentTicker) + HistoricalIdBase;
            logger.LogDebug("Request: {0}", id);
            historicalDataStreams[id] = historicalData;
            IbClient.ClientSocket.ReqHistoricalData(id, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRth, 1, keepUpToDate, new List<TagValue>());
            return historicalData;
        }

        private void UpdateUi(HistoricalDataMessage message)
        {
            logger.LogDebug("Next message: {0}", message.RequestId);
            GetStream(message.RequestId)?.OnNext(message);
        }

        private Subject<HistoricalDataMessage> GetStream(int id)
        {
            if (!historicalDataStreams.TryGetValue(id, out var stream))
            {
                logger.LogDebug("Unknown request: {0}", id);
            }

            return stream;
        }

        private void OnCompleted(HistoricalDataEndMessage message)
        {
            logger.LogDebug("Next message: {0}", message.RequestId);
            GetStream(message.RequestId)?.OnCompleted();
            historicalDataStreams.TryRemove(message.RequestId, out var stream);
            stream?.Dispose();
        }

        public override void Dispose()
        {
            logger.LogDebug("Disposing");
            IbClient.HistoricalData -= UpdateUi;
            IbClient.HistoricalDataUpdate -= UpdateUi;
            IbClient.HistoricalDataEnd -= OnCompleted;
            foreach (var dataStream in historicalDataStreams)
            {
                dataStream.Value.Dispose();
            }
        }
    }
}
