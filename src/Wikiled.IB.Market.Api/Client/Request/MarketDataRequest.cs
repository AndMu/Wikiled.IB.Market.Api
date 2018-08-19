using System;
using Wikiled.IB.Market.Api.Client.Types;

namespace Wikiled.IB.Market.Api.Client.Request
{
    public class MarketDataRequest
    {
        public MarketDataRequest(Contract contract, DateTime endDateTime, Duration duration, BarSize barSize, WhatToShow whatToShow)
        {
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
            EndDateTime = endDateTime;
            Duration = duration;
            BarSize = barSize;
            WhatToShow = whatToShow;
        }

        public Contract Contract{ get; }

        public DateTime EndDateTime { get; }

        public Duration Duration { get; }

        public BarSize BarSize { get; }

        public WhatToShow WhatToShow { get; }

        public bool UseRth { get; set; }

        public int Rth => UseRth ? 1 : 0;

        public int DateFormat { get; set; } = 1;

        public bool KeepUpToDate { get; set; }
    }
}
