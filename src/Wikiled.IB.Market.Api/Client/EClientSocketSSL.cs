using System.IO;
using System.Net.Security;

namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Implements a Secure Socket Layer (SSL) on top of the EClientSocket class. 
    */
    public class EClientSocketSsl : EClientSocket
    {
        public EClientSocketSsl(IEWrapper wrapper, IEReaderSignal signal)
            :
            base(wrapper, signal)
        {
        }

        protected override Stream CreateClientStream(string host, int port)
        {
            var rval = new SslStream(base.CreateClientStream(host, port), false, (o, cert, chain, errors) => true);

            rval.AuthenticateAsClient(host);

            return rval;
        }
    }
}