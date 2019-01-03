using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Captures incoming messages to the API client and places them into a queue.
    */
    public class EReader : IDisposable
    {
        private const int DefaultInBufSize = ushort.MaxValue / 8;

        private static readonly IEWrapper DefaultWrapper = new DefaultEWrapper();

        private readonly EClientSocket eClientSocket;

        private readonly IEReaderSignal eReaderSignal;

        private readonly List<byte> inBuf = new List<byte>(DefaultInBufSize);

        private readonly ConcurrentQueue<EMessage> msgQueue = new ConcurrentQueue<EMessage>();

        private readonly EDecoder processMsgsDecoder;

        private bool isDisposed;

        private Thread readerThread;

        public EReader(EClientSocket clientSocket, IEReaderSignal signal)
        {
            eClientSocket = clientSocket;
            eReaderSignal = signal;
            processMsgsDecoder = new EDecoder(eClientSocket.ServerVersion, eClientSocket.Wrapper, eClientSocket);
        }

        private bool UseV100Plus => eClientSocket.UseV100Plus;

        public void Dispose()
        {
            isDisposed = true;
        }

        public void Start()
        {
            readerThread = new Thread(
                               () =>
                               {
                                   try
                                   {
                                       while (eClientSocket.IsConnected && !isDisposed)
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
                               })
                           {
                               IsBackground = true
                           };
            readerThread.Name = "Reader Thread";
            readerThread.Start();
        }

        private EMessage GetMsg()
        {
            msgQueue.TryDequeue(out var message);
            return message;
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

                msgQueue.Enqueue(msg);
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
                    msgSize = new EDecoder(eClientSocket.ServerVersion, DefaultWrapper).ParseAndProcessMsg(inBuf.ToArray());
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