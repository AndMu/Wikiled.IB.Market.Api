using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class EClientSocket
     * @brief TWS/Gateway client class
     * This client class contains all the available methods to communicate with IB. Up to 32 clients can be connected to a single instance of the TWS/Gateway simultaneously. From herein, the TWS/Gateway will be referred to as the Host.
     */
    public class EClientSocket : EClient, IEClientMsgSink
    {
        private readonly IEReaderSignal eReaderSignal;
        private int port;
        private int redirectCount;

        public EClientSocket(IEWrapper Wrapper, IEReaderSignal eReaderSignal)
            :
            base(Wrapper)
        {
            this.eReaderSignal = eReaderSignal;
        }

        void IEClientMsgSink.ServerVersion(int version, string time)
        {
            base.ServerVersion = version;

            if (!UseV100Plus)
            {
                if (!CheckServerVersion(MinServerVer.MinVersion, ""))
                {
                    ReportUpdateTws("");
                    return;
                }
            }
            else if (ServerVersion < Constants.MinVersion || ServerVersion > Constants.MaxVersion)
            {
                Wrapper.Error(ClientId,
                              EClientErrors.UnsupportedVersion.Code,
                              EClientErrors.UnsupportedVersion.Message);
                return;
            }

            if (ServerVersion >= 3)
            {
                if (ServerVersion < MinServerVer.Linking)
                {
                    var buf = new List<byte>();

                    buf.AddRange(Encoding.UTF8.GetBytes(ClientId.ToString()));
                    buf.Add(Constants.Eol);
                    SocketTransport.Send(new EMessage(buf.ToArray()));
                }
            }

            ServerTime = time;
            IsConnected = true;

            if (!AsyncEConnect)
            {
                StartApi();
            }
        }

        /**
        * @brief Redirects connection to different host. 
        */
        public void Redirect(string host)
        {
            if (!AllowRedirect)
            {
                Wrapper.Error(ClientId, EClientErrors.ConnectFail.Code, EClientErrors.ConnectFail.Message);
                return;
            }

            var srv = host.Split(':');

            if (srv.Length > 1)
            {
                if (!int.TryParse(srv[1], out port))
                {
                    throw new EClientException(EClientErrors.BadMessage);
                }
            }


            ++redirectCount;

            if (redirectCount > Constants.RedirectCountMax)
            {
                EDisconnect();
                Wrapper.Error(ClientId, EClientErrors.ConnectFail.Code, "Redirect count exceeded");
                return;
            }

            EDisconnect(false);
            EConnect(srv[0], port, ClientId, ExtraAuth);
        }

        /**
        * Creates socket connection to TWS/IBG. This earlier version of eConnect does not have extraAuth parameter.
        */
        public void EConnect(string host, int port, int clientId)
        {
            EConnect(host, port, clientId, false);
        }

        protected virtual Stream CreateClientStream(string host, int port)
        {
            return new TcpClient(host, port).GetStream();
        }

        /**
        * @brief Creates socket connection to TWS/IBG.
        */
        public void EConnect(string host, int port, int clientId, bool extraAuth)
        {
            if (IsConnected)
            {
                Wrapper.Error(IncomingMessage.NotValid,
                              EClientErrors.AlreadyConnected.Code,
                              EClientErrors.AlreadyConnected.Message);
                return;
            }

            try
            {
                TcpStream = CreateClientStream(host, port);
                this.port = port;
                SocketTransport = new ESocket(TcpStream);

                ClientId = clientId;
                ExtraAuth = extraAuth;

                SendConnectRequest();

                if (!AsyncEConnect)
                {
                    var eReader = new EReader(this, eReaderSignal);
                    while (ServerVersion == 0 && eReader.PutMessageToQueue())
                    {
                        eReaderSignal.WaitForSignal();
                        eReader.ProcessMsgs();
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                Wrapper.Error(ane);
            }
            catch (SocketException se)
            {
                Wrapper.Error(se);
            }
            catch (EClientException e)
            {
                var cmp = e.Err;
                Wrapper.Error(-1, cmp.Code, cmp.Message);
            }
            catch (Exception e)
            {
                Wrapper.Error(e);
            }
        }

        protected override uint PrepareBuffer(BinaryWriter paramsList)
        {
            var rval = (uint)paramsList.BaseStream.Position;
            if (UseV100Plus)
            {
                paramsList.Write(0);
            }

            return rval;
        }

        protected override void CloseAndSend(BinaryWriter request, uint lengthPos)
        {
            if (UseV100Plus)
            {
                request.Seek((int)lengthPos, SeekOrigin.Begin);
                request.Write(IPAddress.HostToNetworkOrder((int)(request.BaseStream.Length - lengthPos - sizeof(int))));
            }

            request.Seek(0, SeekOrigin.Begin);

            var buf = new MemoryStream();

            request.BaseStream.CopyTo(buf);
            SocketTransport.Send(new EMessage(buf.ToArray()));
        }

        public override void EDisconnect(bool resetState = true)
        {
            if (resetState)
            {
                redirectCount = 0;
            }

            base.EDisconnect(resetState);
        }
    }
}