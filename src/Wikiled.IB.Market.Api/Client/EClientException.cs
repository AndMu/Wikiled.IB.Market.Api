using System;

namespace Wikiled.IB.Market.Api.Client
{
    public class EClientException : Exception
    {
        public EClientException(CodeMsgPair err)
        {
            Err = err;
        }

        public CodeMsgPair Err { get; }
    }
}