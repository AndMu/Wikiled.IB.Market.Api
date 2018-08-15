using System;
using System.IO;

namespace Wikiled.IB.Market.Api.Client
{
    internal class ESocket : IETransport, IDisposable
    {
        private readonly BinaryWriter tcpWriter;
        private readonly object tcpWriterLock = new object();

        public ESocket(Stream socketStream)
        {
            tcpWriter = new BinaryWriter(socketStream);
        }

        public void Send(EMessage msg)
        {
            lock (tcpWriterLock)
            {
                tcpWriter.Write(msg.GetBuf());
            }
        }

        public void Dispose()
        {
            tcpWriter.Dispose();
        }
    }
}