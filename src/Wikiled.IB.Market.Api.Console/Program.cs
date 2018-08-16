using System.Threading;
using System.Threading.Tasks;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.DataManagers;

namespace Wikiled.IB.Market.Api.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            EReaderMonitorSignal signal = new EReaderMonitorSignal();
            IBClient ibClient = new IBClient(signal);
            HistoricalDataManager historicalDataManager = new HistoricalDataManager(ibClient);
            ibClient.HistoricalData += historicalDataManager.UpdateUi;
            ibClient.HistoricalDataUpdate += historicalDataManager.UpdateUi;
            ibClient.HistoricalDataEnd += historicalDataManager.UpdateUi;

            int port = 7496;
            string host = "127.0.0.1";
            ibClient.ClientId = 1;
            ibClient.ClientSocket.EConnect(host, port, ibClient.ClientId);
            var reader = new EReader(ibClient.ClientSocket, signal);
            reader.Start();

            new Thread(() =>
            {
                while (ibClient.ClientSocket.IsConnected)
                {
                    signal.WaitForSignal();
                    reader.ProcessMsgs();
                }
            }) { IsBackground = true }.Start();


            Contract contract = GetMDContract();
            string endTime = "20130808 23:59:59 GMT";
            string duration = "10 D";
            string barSize = "1 day";
            string whatToShow = "MIDPOINT";
            int outsideRTH = false ? 1 : 0;
            historicalDataManager.AddRequest(contract, endTime, duration, barSize, whatToShow, outsideRTH, 1, false);
            await Task.Delay(5000).ConfigureAwait(false);
            ibClient.ClientSocket.EDisconnect();
            
        }

        private static Contract GetMDContract()
        {
            Contract contract = new Contract();
            contract.SecType = SecType.STK;
            contract.Symbol = "AMD";
            contract.Exchange = ExchangeType.SMART;
            contract.Currency = "USD";
            //contract.LastTradeDateOrContractMonth = null;
            //contract.PrimaryExch = null;
            //contract.IncludeExpired = includeExpired.Checked;
            //if (!mdContractRight.Text.Equals("") && !mdContractRight.Text.Equals("None"))
            //{
            //    contract.Right = (string)((IBType)mdContractRight.SelectedItem).Value;
            //}
            //contract.Strike = stringToDouble(this.strike_TMD_MDT.Text);
            //contract.Multiplier = this.multiplier_TMD_MDT.Text;
            //contract.LocalSymbol = this.localSymbol_TMD_MDT.Text;

            return contract;
        }
    }
}
