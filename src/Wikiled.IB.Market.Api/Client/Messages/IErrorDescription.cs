namespace Wikiled.IB.Market.Api.Client.Messages
{
    public interface IErrorDescription
    {
        int Id { get; }

        string ErrorMsg { get; }

        int ErrorCode { get; }
    }
}