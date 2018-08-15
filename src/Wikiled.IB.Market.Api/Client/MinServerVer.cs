namespace Wikiled.IB.Market.Api.Client
{
    public class MinServerVer
    {
        public const int MinVersion = 38;

        //shouldn't these all be deprecated?
        public const int HistoricalData = 24;
        public const int CurrentTime = 33;
        public const int RealTimeBars = 34;
        public const int ScaleOrders = 35;
        public const int SnapshotMktData = 35;
        public const int SshortComboLegs = 35;
        public const int WhatIfOrders = 36;
        public const int ContractConid = 37;

        public const int PtaOrders = 39;
        public const int FundamentalData = 40;
        public const int DeltaNeutral = 40;
        public const int ContractDataChain = 40;
        public const int ScaleOrders2 = 40;
        public const int AlgoOrders = 41;
        public const int ExecutionDataChain = 42;
        public const int NotHeld = 44;
        public const int SecIdType = 45;
        public const int PlaceOrderConid = 46;
        public const int ReqMktDataConid = 47;
        public const int ReqCalcImpliedVolat = 49;
        public const int ReqCalcOptionPrice = 50;
        public const int CancelCalcImpliedVolat = 50;
        public const int CancelCalcOptionPrice = 50;
        public const int SshortxOld = 51;
        public const int Sshortx = 52;
        public const int ReqGlobalCancel = 53;
        public const int HedgeOrders = 54;
        public const int ReqMarketDataType = 55;
        public const int OptOutSmartRouting = 56;
        public const int SmartComboRoutingParams = 57;
        public const int DeltaNeutralConid = 58;
        public const int ScaleOrders3 = 60;
        public const int OrderComboLegsPrice = 61;
        public const int TrailingPercent = 62;
        public const int DeltaNeutralOpenClose = 66;
        public const int AcctSummary = 67;
        public const int TradingClass = 68;
        public const int ScaleTable = 69;
        public const int Linking = 70;
        public const int AlgoId = 71;
        public const int OptionalCapabilities = 72;
        public const int OrderSolicited = 73;
        public const int LinkingAuth = 74;
        public const int Primaryexch = 75;
        public const int RandomizeSizeAndPrice = 76;
        public const int FractionalPositions = 101;
        public const int PeggedToBenchmark = 102;
        public const int ModelsSupport = 103;
        public const int SecDefOptParamsReq = 104;
        public const int ExtOperator = 105;
        public const int SoftDollarTier = 106;
        public const int ReqFamilyCodes = 107;
        public const int ReqMatchingSymbols = 108;
        public const int PastLimit = 109;
        public const int MdSizeMultiplier = 110;
        public const int CashQty = 111;
        public const int ReqMktDepthExchanges = 112;
        public const int TickNews = 113;
        public const int SmartComponents = 114;
        public const int ReqNewsProviders = 115;
        public const int ReqNewsArticle = 116;
        public const int ReqHistoricalNews = 117;
        public const int ReqHeadTimestamp = 118;
        public const int ReqHistogramData = 119;
        public const int ServiceDataType = 120;
        public const int AggGroup = 121;
        public const int UnderlyingInfo = 122;
        public const int CancelHeadtimestamp = 123;
        public const int SyntRealtimeBars = 124;
        public const int CfdReroute = 125;
        public const int MarketRules = 126;
        public const int Pnl = 127;
        public const int NewsQueryOrigins = 128;
        public const int UnrealizedPnl = 129;
        public const int HistoricalTicks = 130;
        public const int MarketCapPrice = 131;
        public const int PreOpenBidAsk = 132;
        public const int RealExpirationDate = 134;
        public const int RealizedPnl = 135;
        public const int LastLiquidity = 136;
        public const int TickByTick = 137;
        public const int DecisionMaker = 138;
        public const int MifidExecution = 139;
        public const int TickByTickIgnoreSize = 140;
        public const int AutoPriceForHedge = 141;
        public const int WhatIfExtFields = 142;
    }
}