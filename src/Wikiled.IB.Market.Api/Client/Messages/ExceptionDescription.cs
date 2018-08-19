using System;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ExceptionDescription : IErrorDescription
    {
        public ExceptionDescription(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ErrorMsg = Exception.Message;
        }

        public int Id { get; }

        public string ErrorMsg { get; }

        public int ErrorCode => -1;

        public Exception Exception { get; }

        public override string ToString()
        {
            return $"Error: {ErrorMsg}";
        }
    }
}
