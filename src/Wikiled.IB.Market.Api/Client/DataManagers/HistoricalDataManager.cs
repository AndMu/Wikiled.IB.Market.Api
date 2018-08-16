using System.Collections.Generic;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class HistoricalDataManager
    {
        public const int HistoricalIdBase = 30000000;

        private string fullDatePattern = "yyyyMMdd  HH:mm:ss";

        private string yearMonthDayPattern = "yyyyMMdd";

        protected int BarCounter = -1;

        protected int CurrentTicker = 1;

        private List<HistoricalDataMessage> historicalData;

        private IBClient ibClient;

        public HistoricalDataManager(IBClient ibClient)
        {
            this.ibClient = ibClient;
        }

        public void AddRequest(Contract contract, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRth, int dateFormat, bool keepUpToDate)
        {
            Clear();
            ibClient.ClientSocket.ReqHistoricalData(CurrentTicker + HistoricalIdBase, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRth, 1, keepUpToDate, new List<TagValue>());
        }

        public void Clear()
        {
            BarCounter = -1;
            historicalData = new List<HistoricalDataMessage>();
        }

        public void UpdateUi(HistoricalDataMessage message)
        {
            historicalData.Add(message);
        }

        public void UpdateUi(HistoricalDataEndMessage message)
        {
        }
    }
}
