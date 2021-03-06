﻿using System.Collections.Generic;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class SecurityDefinitionOptionParameterMessage
    {
        public int ReqId { get; }

        public string Exchange { get; }

        public int UnderlyingConId { get; }

        public string TradingClass { get; }

        public string Multiplier { get; }

        public HashSet<string> Expirations { get; }

        public HashSet<double> Strikes { get; }

        public SecurityDefinitionOptionParameterMessage(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            ReqId = reqId;
            Exchange = exchange;
            UnderlyingConId = underlyingConId;
            TradingClass = tradingClass;
            Multiplier = multiplier;
            Expirations = expirations;
            Strikes = strikes;
        }
    }
}
