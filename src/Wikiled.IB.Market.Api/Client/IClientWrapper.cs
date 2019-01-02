using System;

namespace Wikiled.IB.Market.Api.Client
{
    public interface IClientWrapper : IDisposable
    {
        TimeZoneInfo TimeZone { get; set; }

        bool Connect();

        void Disconnect();
    }
}