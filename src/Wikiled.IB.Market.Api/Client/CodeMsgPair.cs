namespace Wikiled.IB.Market.Api.Client
{
    public class CodeMsgPair
    {
        public CodeMsgPair(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }
    }
}