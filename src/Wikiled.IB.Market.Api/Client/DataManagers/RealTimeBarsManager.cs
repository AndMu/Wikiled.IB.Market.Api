using System;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class RealTimeBarsManager : HistoricalDataManager
    {
        public const int RT_BARS_ID_BASE = 40000000;

        public RealTimeBarsManager(IBClient ibClient) 
            : base(ibClient)
        {
        }

        public void AddRequest(Contract contract, string whatToShow, bool useRTH)
        {
            Clear();
            IbClient.ClientSocket.ReqRealTimeBars(CurrentTicker + RT_BARS_ID_BASE, contract, 5, whatToShow, useRTH, null);
        }

        public override void Clear()
        {
            IbClient.ClientSocket.CancelRealTimeBars(CurrentTicker + RT_BARS_ID_BASE);
            base.Clear();
        }

        public void UpdateUI(RealTimeBarMessage rtBar)
        {
            BarCounter++;
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime dt = start.AddMilliseconds(rtBar.Timestamp * 1000).ToLocalTime();
        }
    }
}
