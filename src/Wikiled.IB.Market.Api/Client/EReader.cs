using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Captures incoming messages to the API client and places them into a queue.
    */
    public class EReader
    {
        private const int DefaultInBufSize = ushort.MaxValue / 8;


        private static readonly IEWrapper DefaultWrapper = new DefaultEWrapper();
        private readonly EClientSocket eClientSocket;
        private readonly IEReaderSignal eReaderSignal;

        private readonly List<byte> inBuf = new List<byte>(DefaultInBufSize);
        private readonly Queue<EMessage> msgQueue = new Queue<EMessage>();
        private readonly EDecoder processMsgsDecoder;

        public EReader(EClientSocket clientSocket, IEReaderSignal signal)
        {
            eClientSocket = clientSocket;
            eReaderSignal = signal;
            processMsgsDecoder = new EDecoder(eClientSocket.ServerVersion, eClientSocket.Wrapper, eClientSocket);
        }

        private bool UseV100Plus => eClientSocket.UseV100Plus;

        public void Start()
        {
            new Thread(() =>
            {
                try
                {
                    while (eClientSocket.IsConnected)
                    {
                        if (!PutMessageToQueue())
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    eClientSocket.Wrapper.Error(ex);
                    eClientSocket.EDisconnect();
                }

                eReaderSignal.IssueSignal();
            }) { IsBackground = true }.Start();
        }

        private EMessage GetMsg()
        {
            lock (msgQueue)
            {
                return msgQueue.Count == 0 ? null : msgQueue.Dequeue();
            }
        }

        public void ProcessMsgs()
        {
            var msg = GetMsg();

            while (msg != null && processMsgsDecoder.ParseAndProcessMsg(msg.GetBuf()) > 0)
            {
                msg = GetMsg();
            }
        }

        public bool PutMessageToQueue()
        {
            try
            {
                var msg = ReadSingleMessage();

                if (msg == null)
                {
                    return false;
                }

                lock (msgQueue)
                {
                    msgQueue.Enqueue(msg);
                }

                eReaderSignal.IssueSignal();

                return true;
            }
            catch (Exception ex)
            {
                if (eClientSocket.IsConnected)
                {
                    eClientSocket.Wrapper.Error(ex.Message);
                }

                return false;
            }
        }

        private EMessage ReadSingleMessage()
        {
            var msgSize = 0;

            if (UseV100Plus)
            {
                msgSize = eClientSocket.ReadInt();

                if (msgSize > Constants.MaxMsgSize)
                {
                    throw new EClientException(EClientErrors.BadLength);
                }

                return new EMessage(eClientSocket.ReadByteArray(msgSize));
            }

            if (inBuf.Count == 0)
            {
                AppendInBuf();
            }

            while (true)
            {
                try
                {
                    msgSize =
                        new EDecoder(eClientSocket.ServerVersion, DefaultWrapper).ParseAndProcessMsg(inBuf.ToArray());
                    break;
                }
                catch (EndOfStreamException)
                {
                    if (inBuf.Count >= inBuf.Capacity * 3 / 4)
                    {
                        inBuf.Capacity *= 2;
                    }

                    AppendInBuf();
                }
            }

            var msgBuf = new byte[msgSize];

            inBuf.CopyTo(0, msgBuf, 0, msgSize);
            inBuf.RemoveRange(0, msgSize);

            if (inBuf.Count < DefaultInBufSize && inBuf.Capacity > DefaultInBufSize)
            {
                inBuf.Capacity = DefaultInBufSize;
            }

            return new EMessage(msgBuf);
        }

        private void AppendInBuf()
        {
            inBuf.AddRange(eClientSocket.ReadAtLeastNBytes(inBuf.Capacity - inBuf.Count));
        }
    }
}