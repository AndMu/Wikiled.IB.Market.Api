﻿namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class DeepBookMessage
    {
        public DeepBookMessage(int tickerId, int position, int operation, int side, double price, int size, string marketMaker, bool isSmartDepth)
        {
            RequestId = tickerId;
            Position = position;
            Operation = operation;
            Side = side;
            Price = price;
            Size = size;
            MarketMaker = marketMaker;
            IsSmartDepth = isSmartDepth;
        }

        public int RequestId { get; set; }

        public int Position { get; set; }

        public int Operation { get; set; }

        public int Side { get; set; }

        public double Price { get; set; }

        public int Size { get; set; }

        public string MarketMaker { get; set; }

        public bool IsSmartDepth { get; set; }
    }
}
