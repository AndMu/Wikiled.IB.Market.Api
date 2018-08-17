using System;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ErrorDescription
    {
        public ErrorDescription(int id, int errorCode, string errorMsg)
        {
            Id = id;
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public ErrorDescription(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public int Id { get; }

        public int ErrorCode { get; }

        public string ErrorMsg { get; }

        public Exception Exception { get; }
    }
}
