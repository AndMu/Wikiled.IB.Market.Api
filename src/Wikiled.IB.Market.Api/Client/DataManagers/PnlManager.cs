using System;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class PnLManager
    {
        private readonly IBClient ibClient;
        private int pnlReqId;
        private int pnlSingleReqId;

        public PnLManager(IBClient ibClient)
        {
            this.ibClient = ibClient;
        }

        public void ReqPnL(string account, string modelCode)
        {
            pnlReqId = new Random(DateTime.UtcNow.Millisecond).Next();

            ibClient.ClientSocket.ReqPnL(pnlReqId, account, modelCode);
        }

        public void CancelPnL()
        {
            if (pnlReqId != 0)
            {
                ibClient.ClientSocket.CancelPnL(pnlReqId);
                pnlReqId = 0;
            }
        }

        public void ReqPnLSingle(string account, string modelCode, int conId)
        {
            pnlSingleReqId = new Random(DateTime.Now.Millisecond).Next();

            ibClient.ClientSocket.ReqPnLSingle(pnlSingleReqId, account, modelCode, conId);
        }

        public void CancelPnLSingle()
        {
            if (pnlSingleReqId != 0)
            {
                ibClient.ClientSocket.CancelPnLSingle(pnlSingleReqId);
                pnlSingleReqId = 0;
            }
        }
    }
}