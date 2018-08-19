using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client
{
    public class IBClient : IEWrapper
    {
        public IBClient(IEReaderSignal signal)
        {
            ClientSocket = new EClientSocket(this, signal);
        }

        public int ClientId { get; set; }

        public EClientSocket ClientSocket { get; }

        public int NextOrderId { get; set; }

        void IEWrapper.Error(Exception e)
        {
            Error?.Invoke(new ExceptionDescription(e));
        }

        void IEWrapper.Error(string str)
        {
            Error?.Invoke(new ErrorDescription(0, 0, str));
        }

        void IEWrapper.Error(int id, int errorCode, string errorMsg)
        {
            Error?.Invoke(new ErrorDescription(id, errorCode, errorMsg));
        }

        void IEWrapper.ConnectionClosed()
        {
            ConnectionClosed?.Invoke();
        }

        void IEWrapper.CurrentTime(long time)
        {
            CurrentTime?.Invoke(time);
        }

        void IEWrapper.TickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            TickPrice?.Invoke(new TickPriceMessage(tickerId, field, price, attribs));
        }

        void IEWrapper.TickSize(int tickerId, int field, int size)
        {
            TickSize?.Invoke(new TickSizeMessage(tickerId, field, size));
        }

        void IEWrapper.TickString(int tickerId, int tickType, string value)
        {
            TickString?.Invoke(tickerId, tickType, value);
        }

        void IEWrapper.TickGeneric(int tickerId, int field, double value)
        {
            TickGeneric?.Invoke(tickerId, field, value);
        }

        void IEWrapper.TickEfp(int tickerId,
                               int tickType,
                               double basisPoints,
                               string formattedBasisPoints,
                               double impliedFuture,
                               int holdDays,
                               string futureLastTradeDate,
                               double dividendImpact,
                               double dividendsToLastTradeDate)
        {
            TickEFP?.Invoke(tickerId,
                             tickType,
                             basisPoints,
                             formattedBasisPoints,
                             impliedFuture,
                             holdDays,
                             futureLastTradeDate,
                             dividendImpact,
                             dividendsToLastTradeDate);
        }

        void IEWrapper.TickSnapshotEnd(int tickerId)
        {
            TickSnapshotEnd?.Invoke(tickerId);
        }

        void IEWrapper.NextValidId(int orderId)
        {
            NextValidId?.Invoke(new ConnectionStatusMessage(true));
        }

        void IEWrapper.DeltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
        {
            DeltaNeutralValidation?.Invoke(reqId, deltaNeutralContract);
        }

        void IEWrapper.ManagedAccounts(string accountsList)
        {
            ManagedAccounts?.Invoke(new ManagedAccountsMessage(accountsList));
        }

        void IEWrapper.TickOptionComputation(int tickerId,
                                             int field,
                                             double impliedVolatility,
                                             double delta,
                                             double optPrice,
                                             double pvDividend,
                                             double gamma,
                                             double vega,
                                             double theta,
                                             double undPrice)
        {
            TickOptionCommunication?.Invoke(new TickOptionMessage(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice));
        }

        void IEWrapper.AccountSummary(int reqId, string account, string tag, string value, string currency)
        {
            AccountSummary?.Invoke(new AccountSummaryMessage(reqId, account, tag, value, currency));
        }

        void IEWrapper.AccountSummaryEnd(int reqId)
        {
            AccountSummaryEnd?.Invoke(new AccountSummaryEndMessage(reqId));
        }

        void IEWrapper.UpdateAccountValue(string key, string value, string currency, string accountName)
        {
            UpdateAccountValue?.Invoke(new AccountValueMessage(key, value, currency, accountName));
        }

        void IEWrapper.UpdatePortfolio(Contract contract,
                                       double position,
                                       double marketPrice,
                                       double marketValue,
                                       double averageCost,
                                       double unrealizedPNL,
                                       double realizedPNL,
                                       string accountName)
        {
            UpdatePortfolio?.Invoke(new UpdatePortfolioMessage(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName));
        }

        void IEWrapper.UpdateAccountTime(string timestamp)
        {
            UpdateAccountTime?.Invoke(new UpdateAccountTimeMessage(timestamp));
        }

        void IEWrapper.AccountDownloadEnd(string account)
        {
            AccountDownloadEnd?.Invoke(new AccountDownloadEndMessage(account));
        }

        void IEWrapper.OrderStatus(int orderId,
                                   string status,
                                   double filled,
                                   double remaining,
                                   double avgFillPrice,
                                   int permId,
                                   int parentId,
                                   double lastFillPrice,
                                   int clientId,
                                   string whyHeld,
                                   double mktCapPrice)
        {
            OrderStatus?.Invoke(new OrderStatusMessage(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld, mktCapPrice));
        }

        void IEWrapper.OpenOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            OpenOrder?.Invoke(new OpenOrderMessage(orderId, contract, order, orderState));
        }

        void IEWrapper.OpenOrderEnd()
        {
            OpenOrderEnd?.Invoke();
        }

        void IEWrapper.ContractDetails(int reqId, ContractDetails contractDetails)
        {
            ContractDetails?.Invoke(new ContractDetailsMessage(reqId, contractDetails));
        }

        void IEWrapper.ContractDetailsEnd(int reqId)
        {
            ContractDetailsEnd?.Invoke(reqId);
        }

        void IEWrapper.ExecDetails(int reqId, Contract contract, Execution execution)
        {
            ExecDetails?.Invoke(new ExecutionMessage(reqId, contract, execution));
        }

        void IEWrapper.ExecDetailsEnd(int reqId)
        {
            ExecDetailsEnd?.Invoke(reqId);
        }

        void IEWrapper.CommissionReport(CommissionReport commissionReport)
        {
            CommissionReport?.Invoke(commissionReport);
        }

        void IEWrapper.FundamentalData(int reqId, string data)
        {
            FundamentalData?.Invoke(new FundamentalsMessage(data));
        }

        void IEWrapper.HistoricalData(int requestId, Bar bar)
        {
            HistoricalData?.Invoke(new HistoricalDataMessage(requestId, bar));
        }

        void IEWrapper.HistoricalDataEnd(int reqId, string startDate, string endDate)
        {
            HistoricalDataEnd?.Invoke(new HistoricalDataEndMessage(reqId, startDate, endDate));
        }

        void IEWrapper.MarketDataType(int reqId, int marketDataType)
        {
            MarketDataType?.Invoke(new MarketDataTypeMessage(reqId, marketDataType));
        }

        void IEWrapper.UpdateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            UpdateMktDepth?.Invoke(new DeepBookMessage(tickerId, position, operation, side, price, size, ""));
        }

        void IEWrapper.UpdateMktDepthL2(int tickerId,
                                        int position,
                                        string marketMaker,
                                        int operation,
                                        int side,
                                        double price,
                                        int size)
        {
            UpdateMktDepthL2?.Invoke(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker));
        }

        void IEWrapper.UpdateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            UpdateNewsBulletin?.Invoke(msgId, msgType, message, origExchange);
        }

        void IEWrapper.Position(string account, Contract contract, double pos, double avgCost)
        {
            Position.Invoke(new PositionMessage(account, contract, pos, avgCost));
        }

        void IEWrapper.PositionEnd()
        {
            PositionEnd?.Invoke();
        }

        void IEWrapper.RealtimeBar(int reqId,
                                   long time,
                                   double open,
                                   double high,
                                   double low,
                                   double close,
                                   long volume,
                                   double WAP,
                                   int count)
        {
            RealtimeBar?.Invoke(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count));
        }

        void IEWrapper.ScannerParameters(string xml)
        {
            ScannerParameters?.Invoke(xml);
        }

        void IEWrapper.ScannerData(int reqId,
                                   int rank,
                                   ContractDetails contractDetails,
                                   string distance,
                                   string benchmark,
                                   string projection,
                                   string legsStr)
        {
            ScannerData?.Invoke(new ScannerMessage(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
        }

        void IEWrapper.ScannerDataEnd(int reqId)
        {
            ScannerDataEnd?.Invoke(reqId);
        }

        void IEWrapper.ReceiveFa(int faDataType, string faXmlData)
        {
            ReceiveFA?.Invoke(new AdvisorDataMessage(faDataType, faXmlData));
        }

        void IEWrapper.BondContractDetails(int requestId, ContractDetails contractDetails)
        {
            BondContractDetails?.Invoke(new BondContractDetailsMessage(requestId, contractDetails));
        }

        void IEWrapper.VerifyMessageApi(string ApiData)
        {
            VerifyMessageApi?.Invoke(ApiData);
        }

        void IEWrapper.VerifyCompleted(bool isSuccessful, string errorText)
        {
            VerifyCompleted?.Invoke(isSuccessful, errorText);
        }

        void IEWrapper.VerifyAndAuthMessageApi(string ApiData, string xyzChallenge)
        {
            VerifyAndAuthMessageApi?.Invoke(ApiData, xyzChallenge);
        }

        void IEWrapper.VerifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            VerifyAndAuthCompleted?.Invoke(isSuccessful, errorText);
        }

        void IEWrapper.DisplayGroupList(int reqId, string groups)
        {
            DisplayGroupList?.Invoke(reqId, groups);
        }

        void IEWrapper.DisplayGroupUpdated(int reqId, string contractInfo)
        {
            DisplayGroupUpdated?.Invoke(reqId, contractInfo);
        }


        void IEWrapper.ConnectAck()
        {
            if (ClientSocket.AsyncEConnect)
            {
                ClientSocket.StartApi();
            }
        }

        void IEWrapper.PositionMulti(int reqId,
                                     string account,
                                     string modelCode,
                                     Contract contract,
                                     double pos,
                                     double avgCost)
        {
            PositionMulti?.Invoke(new PositionMultiMessage(reqId, account, modelCode, contract, pos, avgCost));
        }

        void IEWrapper.PositionMultiEnd(int reqId)
        {
            PositionMultiEnd?.Invoke(reqId);
        }

        void IEWrapper.AccountUpdateMulti(int reqId,
                                          string account,
                                          string modelCode,
                                          string key,
                                          string value,
                                          string currency)
        {
            AccountUpdateMulti?.Invoke(new AccountUpdateMultiMessage(reqId, account, modelCode, key, value, currency));
        }

        void IEWrapper.AccountUpdateMultiEnd(int reqId)
        {
            AccountUpdateMultiEnd?.Invoke(reqId);
        }

        void IEWrapper.SecurityDefinitionOptionParameter(int reqId,
                                                         string exchange,
                                                         int underlyingConId,
                                                         string tradingClass,
                                                         string multiplier,
                                                         HashSet<string> expirations,
                                                         HashSet<double> strikes)
        {
            SecurityDefinitionOptionParameter?.Invoke(new SecurityDefinitionOptionParameterMessage(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes));
        }

        void IEWrapper.SecurityDefinitionOptionParameterEnd(int reqId)
        {
            SecurityDefinitionOptionParameterEnd?.Invoke(reqId);
        }

        void IEWrapper.SoftDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            SoftDollarTiers?.Invoke(new SoftDollarTiersMessage(reqId, tiers));
        }

        void IEWrapper.FamilyCodes(FamilyCode[] familyCodes)
        {
            FamilyCodes?.Invoke(familyCodes);
        }

        void IEWrapper.SymbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            SymbolSamples?.Invoke(new SymbolSamplesMessage(reqId, contractDescriptions));
        }

        void IEWrapper.MktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            MktDepthExchanges?.Invoke(depthMktDataDescriptions);
        }

        void IEWrapper.TickNews(int tickerId, long timeStamp,  string providerCode, string articleId, string headline,  string extraData)
        {
            TickNews?.Invoke(new TickNewsMessage(tickerId, timeStamp, providerCode, articleId, headline, extraData));
        }

        void IEWrapper.SmartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            SmartComponents?.Invoke(reqId, theMap);
        }

        void IEWrapper.TickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            TickReqParams?.Invoke(new TickReqParamsMessage(tickerId, minTick, bboExchange, snapshotPermissions));
        }

        void IEWrapper.NewsProviders(NewsProvider[] newsProviders)
        {
            NewsProviders?.Invoke(new NewsProvidersMessage(newsProviders));
        }

        void IEWrapper.NewsArticle(int requestId, int articleType, string articleText)
        {
            NewsArticle?.Invoke(new NewsArticleMessage(requestId, articleType, articleText));
        }

        void IEWrapper.HistoricalNews(int requestId,
                                      string time,
                                      string providerCode,
                                      string articleId,
                                      string headline)
        {
            HistoricalNews?.Invoke(new HistoricalNewsMessage(requestId, time, providerCode, articleId, headline));
        }

        void IEWrapper.HistoricalNewsEnd(int requestId, bool hasMore)
        {
            HistoricalNewsEnd?.Invoke(new HistoricalNewsEndMessage(requestId, hasMore));
        }

        void IEWrapper.HeadTimestamp(int reqId, string headTimestamp)
        {
            HeadTimestamp?.Invoke(new HeadTimestampMessage(reqId, headTimestamp));
        }

        void IEWrapper.HistogramData(int reqId, HistogramEntry[] data)
        {
            HistogramData?.Invoke(new HistogramDataMessage(reqId, data));
        }

        void IEWrapper.HistoricalDataUpdate(int reqId, Bar bar)
        {
            HistoricalDataUpdate?.Invoke(new HistoricalDataMessage(reqId, bar));
        }

        void IEWrapper.RerouteMktDataReq(int reqId, int conId, string exchange)
        {
            RerouteMktDataReq?.Invoke(reqId, conId, exchange);
        }

        void IEWrapper.RerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            RerouteMktDepthReq?.Invoke(reqId, conId, exchange);
        }

        void IEWrapper.MarketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            MarketRule?.Invoke(new MarketRuleMessage(marketRuleId, priceIncrements));
        }

        void IEWrapper.Pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            pnl?.Invoke(new PnLMessage(reqId, dailyPnL, unrealizedPnL, realizedPnL));
        }

        void IEWrapper.PnlSingle(int reqId,
                                 int pos,
                                 double dailyPnL,
                                 double unrealizedPnL,
                                 double realizedPnL,
                                 double value)
        {
            pnlSingle?.Invoke(new PnLSingleMessage(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value));
        }

        void IEWrapper.HistoricalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            ticks.ToList()
                .ForEach(tick => historicalTick?.Invoke(new HistoricalTickMessage(reqId, tick.Time, tick.Price, tick.Size)));
        }

        void IEWrapper.HistoricalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            ticks.ToList()
                .ForEach(tick => historicalTickBidAsk?.Invoke(new HistoricalTickBidAskMessage(
                                                     reqId,
                                                     tick.Time,
                                                     tick.Mask,
                                                     tick.PriceBid,
                                                     tick.PriceAsk,
                                                     tick.SizeBid,
                                                     tick.SizeAsk)));
        }

        void IEWrapper.HistoricalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            ticks.ToList()
                .ForEach(tick => historicalTickLast?.Invoke(new HistoricalTickLastMessage(
                                                     reqId,
                                                     tick.Time,
                                                     tick.Mask,
                                                     tick.Price,
                                                     tick.Size,
                                                     tick.Exchange,
                                                     tick.SpecialConditions)));
        }

        void IEWrapper.TickByTickAllLast(int reqId,
                                         int tickType,
                                         long time,
                                         double price,
                                         int size,
                                         TickAttrib attribs,
                                         string exchange,
                                         string specialConditions)
        {
            tickByTickAllLast?.Invoke(new TickByTickAllLastMessage(reqId, tickType, time, price, size, attribs, exchange, specialConditions));
        }

        void IEWrapper.TickByTickBidAsk(int reqId,
                                        long time,
                                        double bidPrice,
                                        double askPrice,
                                        int bidSize,
                                        int askSize,
                                        TickAttrib attribs)
        {
            tickByTickBidAsk?.Invoke(new TickByTickBidAskMessage(reqId, time, bidPrice, askPrice, bidSize, askSize, attribs));
        }

        void IEWrapper.TickByTickMidPoint(int reqId, long time, double midPoint)
        {
            tickByTickMidPoint?.Invoke(new TickByTickMidPointMessage(reqId, time, midPoint));
        }
     
        public event Action<IErrorDescription> Error;

        public event Action ConnectionClosed;

        public event Action<long> CurrentTime;

        public event Action<TickPriceMessage> TickPrice;

        public event Action<TickSizeMessage> TickSize;

        public event Action<int, int, string> TickString;

        public event Action<int, int, double> TickGeneric;

        public event Action<int, int, double, string, double, int, string, double, double> TickEFP;

        public event Action<int> TickSnapshotEnd;

        public event Action<ConnectionStatusMessage> NextValidId;

        public event Action<int, DeltaNeutralContract> DeltaNeutralValidation;

        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public event Action<TickOptionMessage> TickOptionCommunication;

        public event Action<AccountSummaryMessage> AccountSummary;

        public event Action<AccountSummaryEndMessage> AccountSummaryEnd;

        public event Action<AccountValueMessage> UpdateAccountValue;

        public event Action<UpdatePortfolioMessage> UpdatePortfolio;

        public event Action<UpdateAccountTimeMessage> UpdateAccountTime;

        public event Action<AccountDownloadEndMessage> AccountDownloadEnd;

        public event Action<OrderStatusMessage> OrderStatus;

        public event Action<OpenOrderMessage> OpenOrder;

        public event Action OpenOrderEnd;

        public event Action<ContractDetailsMessage> ContractDetails;

        public event Action<int> ContractDetailsEnd;

        public event Action<ExecutionMessage> ExecDetails;

        public event Action<int> ExecDetailsEnd;

        public event Action<CommissionReport> CommissionReport;

        public event Action<FundamentalsMessage> FundamentalData;

        public event Action<HistoricalDataMessage> HistoricalData;

        public event Action<HistoricalDataEndMessage> HistoricalDataEnd;

        public event Action<MarketDataTypeMessage> MarketDataType;

        public event Action<DeepBookMessage> UpdateMktDepth;

        public event Action<DeepBookMessage> UpdateMktDepthL2;

        public event Action<int, int, string, string> UpdateNewsBulletin;

        public event Action<PositionMessage> Position;

        public event Action PositionEnd;

        public event Action<RealTimeBarMessage> RealtimeBar;

        public event Action<string> ScannerParameters;

        public event Action<ScannerMessage> ScannerData;

        public event Action<int> ScannerDataEnd;

        public event Action<AdvisorDataMessage> ReceiveFA;

        public event Action<BondContractDetailsMessage> BondContractDetails;

        public event Action<string> VerifyMessageApi;

        public event Action<bool, string> VerifyCompleted;

        public event Action<string, string> VerifyAndAuthMessageApi;

        public event Action<bool, string> VerifyAndAuthCompleted;

        public event Action<int, string> DisplayGroupList;

        public event Action<int, string> DisplayGroupUpdated;

        public event Action<PositionMultiMessage> PositionMulti;

        public event Action<int> PositionMultiEnd;

        public event Action<AccountUpdateMultiMessage> AccountUpdateMulti;

        public event Action<int> AccountUpdateMultiEnd;

        public event Action<SecurityDefinitionOptionParameterMessage> SecurityDefinitionOptionParameter;

        public event Action<int> SecurityDefinitionOptionParameterEnd;

        public event Action<SoftDollarTiersMessage> SoftDollarTiers;

        public event Action<FamilyCode[]> FamilyCodes;

        public event Action<SymbolSamplesMessage> SymbolSamples;


        public event Action<DepthMktDataDescription[]> MktDepthExchanges;

        public event Action<TickNewsMessage> TickNews;

        public event Action<int, Dictionary<int, KeyValuePair<string, char>>> SmartComponents;

        public event Action<TickReqParamsMessage> TickReqParams;

        public event Action<NewsProvidersMessage> NewsProviders;

        public event Action<NewsArticleMessage> NewsArticle;

        public event Action<HistoricalNewsMessage> HistoricalNews;

        public event Action<HistoricalNewsEndMessage> HistoricalNewsEnd;

        public event Action<HeadTimestampMessage> HeadTimestamp;

        public event Action<HistogramDataMessage> HistogramData;

        public event Action<HistoricalDataMessage> HistoricalDataUpdate;

        public event Action<int, int, string> RerouteMktDataReq;

        public event Action<int, int, string> RerouteMktDepthReq;

        public event Action<MarketRuleMessage> MarketRule;

        public event Action<PnLMessage> pnl;

        public event Action<PnLSingleMessage> pnlSingle;

        public event Action<HistoricalTickMessage> historicalTick;

        public event Action<HistoricalTickBidAskMessage> historicalTickBidAsk;

        public event Action<HistoricalTickLastMessage> historicalTickLast;

        public event Action<TickByTickAllLastMessage> tickByTickAllLast;

        public event Action<TickByTickBidAskMessage> tickByTickBidAsk;

        public event Action<TickByTickMidPointMessage> tickByTickMidPoint;
    }
}