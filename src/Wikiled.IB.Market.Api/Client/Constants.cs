namespace Wikiled.IB.Market.Api.Client
{
    public static class Constants
    {
        public const int ClientVersion = 66; //API v. 9.71
        public const byte Eol = 0;
        public const int RedirectCountMax = 2;

        public const int FaGroups = 1;
        public const int FaProfiles = 2;
        public const int FaAliases = 3;
        public const int MinVersion = 100;
        public const int MaxVersion = MinServerVer.WhatIfExtFields;
        public const int MaxMsgSize = 0x00FFFFFF;
    }
}