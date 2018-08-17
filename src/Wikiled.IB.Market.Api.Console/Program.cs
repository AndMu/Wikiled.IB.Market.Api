using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client;

namespace Wikiled.IB.Market.Api.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var client = new IBClientWrapper(new LoggerFactory()))
            {
                client.Connect("127.0.0.1", 7496, 1);
                string endTime = "20130808 23:59:59 GMT";
                string duration = "2 D";
                string barSize = "1 day";
                string whatToShow = "MIDPOINT";
                int outsideRTH = false ? 1 : 0;
                var amd = client.GetHistoricalManager().Request(GetMDContract("AMD"), endTime, duration, barSize, whatToShow, outsideRTH, 1, false);
                var ms = client.GetHistoricalManager().Request(GetMDContract("MSFT"), endTime, duration, barSize, whatToShow, outsideRTH, 1, false);
                var dataAmd = await amd.ToArray();
                var dataMS = await ms.ToArray();
            }
        }

        private static Contract GetMDContract(string stock)
        {
            Contract contract = new Contract();
            contract.SecType = SecType.STK;
            contract.Symbol = stock;
            contract.Exchange = ExchangeType.ISLAND;
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
