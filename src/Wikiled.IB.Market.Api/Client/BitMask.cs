/* Copyright (C) 2018 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;

namespace Wikiled.IB.Market.Api.Client
{
    internal class BitMask
    {
        private int mask;

        public BitMask(int p)
        {
            mask = p;
        }

        public bool this[int index]
        {
            get
            {
                if (index >= 32)
                {
                    throw new IndexOutOfRangeException();
                }

                return (mask & (1 << index)) != 0;
            }
            set
            {
                if (index >= 32)
                {
                    throw new IndexOutOfRangeException();
                }

                if (value)
                {
                    mask |= 1 << index;
                }
                else
                {
                    mask &= ~(1 << index);
                }
            }
        }

        public int GetMask()
        {
            return mask;
        }

        public void Clear()
        {
            mask = 0;
        }
    }
}