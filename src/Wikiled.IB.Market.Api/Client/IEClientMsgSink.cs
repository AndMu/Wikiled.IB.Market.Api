namespace Wikiled.IB.Market.Api.Client
{
    internal interface IEClientMsgSink
    {
        void ServerVersion(int version, string time);
        void Redirect(string host);
    }
}