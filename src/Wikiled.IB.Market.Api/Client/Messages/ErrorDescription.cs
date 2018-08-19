using System;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ErrorDescription : IErrorDescription
    {
        public ErrorDescription(int id, int errorCode, string errorMsg)
        {
            Id = id;
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public int Id { get; }

        public int ErrorCode { get; }

        public string ErrorMsg { get; }

        public override string ToString()
        {
            return $"Error: {Id}:{ErrorCode}:{ErrorMsg}";
        }
    }
}
