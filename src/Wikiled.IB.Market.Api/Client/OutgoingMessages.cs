namespace Wikiled.IB.Market.Api.Client
{
    public enum OutgoingMessages
    {
        RequestMarketData = 1,
        CancelMarketData = 2,
        PlaceOrder = 3,
        CancelOrder = 4,
        RequestOpenOrders = 5,
        RequestAccountData = 6,
        RequestExecutions = 7,
        RequestIds = 8,
        RequestContractData = 9,
        RequestMarketDepth = 10,
        CancelMarketDepth = 11,
        RequestNewsBulletins = 12,
        CancelNewsBulletin = 13,
        ChangeServerLog = 14,
        RequestAutoOpenOrders = 15,
        RequestAllOpenOrders = 16,
        RequestManagedAccounts = 17,
        RequestFa = 18,
        ReplaceFa = 19,
        RequestHistoricalData = 20,
        ExerciseOptions = 21,
        RequestScannerSubscription = 22,
        CancelScannerSubscription = 23,
        RequestScannerParameters = 24,
        CancelHistoricalData = 25,
        RequestCurrentTime = 49,
        RequestRealTimeBars = 50,
        CancelRealTimeBars = 51,
        RequestFundamentalData = 52,
        CancelFundamentalData = 53,
        ReqCalcImpliedVolat = 54,
        ReqCalcOptionPrice = 55,
        CancelImpliedVolatility = 56,
        CancelOptionPrice = 57,
        RequestGlobalCancel = 58,
        RequestMarketDataType = 59,
        RequestPositions = 61,
        RequestAccountSummary = 62,
        CancelAccountSummary = 63,
        CancelPositions = 64,
        VerifyRequest = 65,
        VerifyMessage = 66,
        QueryDisplayGroups = 67,
        SubscribeToGroupEvents = 68,
        UpdateDisplayGroup = 69,
        UnsubscribeFromGroupEvents = 70,
        StartApi = 71,
        VerifyAndAuthRequest = 72,
        VerifyAndAuthMessage = 73,
        RequestPositionsMulti = 74,
        CancelPositionsMulti = 75,
        RequestAccountUpdatesMulti = 76,
        CancelAccountUpdatesMulti = 77,
        RequestSecurityDefinitionOptionalParameters = 78,
        RequestSoftDollarTiers = 79,
        RequestFamilyCodes = 80,
        RequestMatchingSymbols = 81,
        RequestMktDepthExchanges = 82,
        RequestSmartComponents = 83,
        RequestNewsArticle = 84,
        RequestNewsProviders = 85,
        RequestHistoricalNews = 86,
        RequestHeadTimestamp = 87,
        RequestHistogramData = 88,
        CancelHistogramData = 89,
        CancelHeadTimestamp = 90,
        RequestMarketRule = 91,
        ReqPnL = 92,
        CancelPnL = 93,
        ReqPnLSingle = 94,
        CancelPnLSingle = 95,
        ReqHistoricalTicks = 96,
        ReqTickByTickData = 97,
        CancelTickByTickData = 98,
        ReqCompletedOrders = 99
    }
}