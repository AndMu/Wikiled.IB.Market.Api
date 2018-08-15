namespace Wikiled.IB.Market.Api.Client
{
    public interface IETransport
    {
        void Send(EMessage msg);
    }
}