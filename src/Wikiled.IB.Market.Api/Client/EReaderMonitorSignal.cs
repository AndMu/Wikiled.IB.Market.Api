using System.Threading;

namespace Wikiled.IB.Market.Api.Client
{
    public class EReaderMonitorSignal : IEReaderSignal
    {
        private readonly object cs = new object();

        private bool open;

        public void IssueSignal()
        {
            lock (cs)
            {
                open = true;

                Monitor.PulseAll(cs);
            }
        }

        public void WaitForSignal()
        {
            lock (cs)
            {
                while (!open)
                {
                    Monitor.Wait(cs);
                }

                open = false;
            }
        }
    }
}