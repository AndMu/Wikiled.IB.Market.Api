/* Copyright (C) 2018 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class TickReqParamsMessage
    {
        public int TickerId { get; }

        public double MinTick { get; }

        public string BboExchange { get; }

        public int SnapshotPermissions { get; }

        public TickReqParamsMessage(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            TickerId = tickerId;
            MinTick = minTick;
            BboExchange = bboExchange;
            SnapshotPermissions = snapshotPermissions;
        }
    }
}
