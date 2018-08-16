using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client
{
    public class IBClient : IEWrapper
    {
        private readonly SynchronizationContext sc;

        public IBClient(IEReaderSignal signal)
        {
            ClientSocket = new EClientSocket(this, signal);
            sc = SynchronizationContext.Current;
        }

        public int ClientId { get; set; }

        public EClientSocket ClientSocket { get; }

        public int NextOrderId { get; set; }

        void IEWrapper.Error(Exception e)
        {
            var tmp = Error;

            if (tmp != null)
            {
                sc.Post(t => tmp(0, 0, null, e), null);
            }
        }

        void IEWrapper.Error(string str)
        {
            var tmp = Error;

            if (tmp != null)
            {
                sc.Post(t => tmp(0, 0, str, null), null);
            }
        }

        void IEWrapper.Error(int id, int errorCode, string errorMsg)
        {
            var tmp = Error;

            if (tmp != null)
            {
                sc.Post(t => tmp(id, errorCode, errorMsg, null), null);
            }
        }

        void IEWrapper.ConnectionClosed()
        {
            var tmp = ConnectionClosed;

            if (tmp != null)
            {
                sc.Post(t => tmp(), null);
            }
        }

        void IEWrapper.CurrentTime(long time)
        {
            var tmp = CurrentTime;

            if (tmp != null)
            {
                sc.Post(t => tmp(time), null);
            }
        }

        void IEWrapper.TickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            var tmp = TickPrice;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickPriceMessage(tickerId, field, price, attribs)), null);
            }
        }

        void IEWrapper.TickSize(int tickerId, int field, int size)
        {
            var tmp = TickSize;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickSizeMessage(tickerId, field, size)), null);
            }
        }

        void IEWrapper.TickString(int tickerId, int tickType, string value)
        {
            var tmp = TickString;

            if (tmp != null)
            {
                sc.Post(t => tmp(tickerId, tickType, value), null);
            }
        }

        void IEWrapper.TickGeneric(int tickerId, int field, double value)
        {
            var tmp = TickGeneric;

            if (tmp != null)
            {
                sc.Post(t => tmp(tickerId, field, value), null);
            }
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
            var tmp = TickEFP;

            if (tmp != null)
            {
                sc.Post(t => tmp(tickerId,
                                 tickType,
                                 basisPoints,
                                 formattedBasisPoints,
                                 impliedFuture,
                                 holdDays,
                                 futureLastTradeDate,
                                 dividendImpact,
                                 dividendsToLastTradeDate),
                        null);
            }
        }

        void IEWrapper.TickSnapshotEnd(int tickerId)
        {
            var tmp = TickSnapshotEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(tickerId), null);
            }
        }

        void IEWrapper.NextValidId(int orderId)
        {
            var tmp = NextValidId;

            if (tmp != null)
            {
                sc.Post(t => tmp(new ConnectionStatusMessage(true)), null);
            }

            NextOrderId = orderId;
        }

        void IEWrapper.DeltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
        {
            var tmp = DeltaNeutralValidation;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, deltaNeutralContract), null);
            }
        }

        void IEWrapper.ManagedAccounts(string accountsList)
        {
            var tmp = ManagedAccounts;

            if (tmp != null)
            {
                sc.Post(t => tmp(new ManagedAccountsMessage(accountsList)), null);
            }
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
            var tmp = TickOptionCommunication;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickOptionMessage(tickerId,
                                                       field,
                                                       impliedVolatility,
                                                       delta,
                                                       optPrice,
                                                       pvDividend,
                                                       gamma,
                                                       vega,
                                                       theta,
                                                       undPrice)),
                        null);
            }
        }

        void IEWrapper.AccountSummary(int reqId, string account, string tag, string value, string currency)
        {
            var tmp = AccountSummary;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AccountSummaryMessage(reqId, account, tag, value, currency)), null);
            }
        }

        void IEWrapper.AccountSummaryEnd(int reqId)
        {
            var tmp = AccountSummaryEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AccountSummaryEndMessage(reqId)), null);
            }
        }

        void IEWrapper.UpdateAccountValue(string key, string value, string currency, string accountName)
        {
            var tmp = UpdateAccountValue;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AccountValueMessage(key, value, currency, accountName)), null);
            }
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
            var tmp = UpdatePortfolio;

            if (tmp != null)
            {
                sc.Post(t => tmp(new UpdatePortfolioMessage(contract,
                                                            position,
                                                            marketPrice,
                                                            marketValue,
                                                            averageCost,
                                                            unrealizedPNL,
                                                            realizedPNL,
                                                            accountName)),
                        null);
            }
        }

        void IEWrapper.UpdateAccountTime(string timestamp)
        {
            var tmp = UpdateAccountTime;

            if (tmp != null)
            {
                sc.Post(t => tmp(new UpdateAccountTimeMessage(timestamp)), null);
            }
        }

        void IEWrapper.AccountDownloadEnd(string account)
        {
            var tmp = AccountDownloadEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AccountDownloadEndMessage(account)), null);
            }
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
            var tmp = OrderStatus;

            if (tmp != null)
            {
                sc.Post(t => tmp(new OrderStatusMessage(orderId,
                                                        status,
                                                        filled,
                                                        remaining,
                                                        avgFillPrice,
                                                        permId,
                                                        parentId,
                                                        lastFillPrice,
                                                        clientId,
                                                        whyHeld,
                                                        mktCapPrice)),
                        null);
            }
        }

        void IEWrapper.OpenOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            var tmp = OpenOrder;

            if (tmp != null)
            {
                sc.Post(t => tmp(new OpenOrderMessage(orderId, contract, order, orderState)), null);
            }
        }

        void IEWrapper.OpenOrderEnd()
        {
            var tmp = OpenOrderEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(), null);
            }
        }

        void IEWrapper.ContractDetails(int reqId, ContractDetails contractDetails)
        {
            var tmp = ContractDetails;

            if (tmp != null)
            {
                sc.Post(t => tmp(new ContractDetailsMessage(reqId, contractDetails)), null);
            }
        }

        void IEWrapper.ContractDetailsEnd(int reqId)
        {
            var tmp = ContractDetailsEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.ExecDetails(int reqId, Contract contract, Execution execution)
        {
            var tmp = ExecDetails;

            if (tmp != null)
            {
                sc.Post(t => tmp(new ExecutionMessage(reqId, contract, execution)), null);
            }
        }

        void IEWrapper.ExecDetailsEnd(int reqId)
        {
            var tmp = ExecDetailsEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.CommissionReport(CommissionReport commissionReport)
        {
            var tmp = CommissionReport;

            if (tmp != null)
            {
                sc.Post(t => tmp(commissionReport), null);
            }
        }

        void IEWrapper.FundamentalData(int reqId, string data)
        {
            var tmp = FundamentalData;

            if (tmp != null)
            {
                sc.Post(t => tmp(new FundamentalsMessage(data)), null);
            }
        }

        void IEWrapper.HistoricalData(int reqId, Bar bar)
        {
            var tmp = HistoricalData;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistoricalDataMessage(reqId, bar)), null);
            }
        }

        void IEWrapper.HistoricalDataEnd(int reqId, string startDate, string endDate)
        {
            var tmp = HistoricalDataEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistoricalDataEndMessage(reqId, startDate, endDate)), null);
            }
        }

        void IEWrapper.MarketDataType(int reqId, int marketDataType)
        {
            var tmp = MarketDataType;

            if (tmp != null)
            {
                sc.Post(t => tmp(new MarketDataTypeMessage(reqId, marketDataType)), null);
            }
        }

        void IEWrapper.UpdateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            var tmp = UpdateMktDepth;
            if (tmp != null)
            {
                sc.Post(t => tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, "")), null);
            }
        }

        void IEWrapper.UpdateMktDepthL2(int tickerId,
                                        int position,
                                        string marketMaker,
                                        int operation,
                                        int side,
                                        double price,
                                        int size)
        {
            var tmp = UpdateMktDepthL2;

            if (tmp != null)
            {
                sc.Post(t => tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker)),
                        null);
            }
        }

        void IEWrapper.UpdateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            var tmp = UpdateNewsBulletin;

            if (tmp != null)
            {
                sc.Post(t => tmp(msgId, msgType, message, origExchange), null);
            }
        }

        void IEWrapper.Position(string account, Contract contract, double pos, double avgCost)
        {
            var tmp = Position;

            if (tmp != null)
            {
                sc.Post(t => tmp(new PositionMessage(account, contract, pos, avgCost)), null);
            }
        }

        void IEWrapper.PositionEnd()
        {
            var tmp = PositionEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(), null);
            }
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
            var tmp = RealtimeBar;

            if (tmp != null)
            {
                sc.Post(t => tmp(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count)),
                        null);
            }
        }

        void IEWrapper.ScannerParameters(string xml)
        {
            var tmp = ScannerParameters;

            if (tmp != null)
            {
                sc.Post(t => tmp(xml), null);
            }
        }

        void IEWrapper.ScannerData(int reqId,
                                   int rank,
                                   ContractDetails contractDetails,
                                   string distance,
                                   string benchmark,
                                   string projection,
                                   string legsStr)
        {
            var tmp = ScannerData;

            if (tmp != null)
            {
                sc.Post(t => tmp(new ScannerMessage(reqId,
                                                    rank,
                                                    contractDetails,
                                                    distance,
                                                    benchmark,
                                                    projection,
                                                    legsStr)),
                        null);
            }
        }

        void IEWrapper.ScannerDataEnd(int reqId)
        {
            var tmp = ScannerDataEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.ReceiveFa(int faDataType, string faXmlData)
        {
            var tmp = ReceiveFA;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AdvisorDataMessage(faDataType, faXmlData)), null);
            }
        }

        void IEWrapper.BondContractDetails(int requestId, ContractDetails contractDetails)
        {
            var tmp = BondContractDetails;

            if (tmp != null)
            {
                sc.Post(t => tmp(new BondContractDetailsMessage(requestId, contractDetails)), null);
            }
        }

        void IEWrapper.VerifyMessageApi(string ApiData)
        {
            var tmp = VerifyMessageApi;

            if (tmp != null)
            {
                sc.Post(t => tmp(ApiData), null);
            }
        }

        void IEWrapper.VerifyCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyCompleted;

            if (tmp != null)
            {
                sc.Post(t => tmp(isSuccessful, errorText), null);
            }
        }

        void IEWrapper.VerifyAndAuthMessageApi(string ApiData, string xyzChallenge)
        {
            var tmp = VerifyAndAuthMessageApi;

            if (tmp != null)
            {
                sc.Post(t => tmp(ApiData, xyzChallenge), null);
            }
        }

        void IEWrapper.VerifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyAndAuthCompleted;

            if (tmp != null)
            {
                sc.Post(t => tmp(isSuccessful, errorText), null);
            }
        }

        void IEWrapper.DisplayGroupList(int reqId, string groups)
        {
            var tmp = DisplayGroupList;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, groups), null);
            }
        }

        void IEWrapper.DisplayGroupUpdated(int reqId, string contractInfo)
        {
            var tmp = DisplayGroupUpdated;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, contractInfo), null);
            }
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
            var tmp = PositionMulti;

            if (tmp != null)
            {
                sc.Post(t => tmp(new PositionMultiMessage(reqId, account, modelCode, contract, pos, avgCost)), null);
            }
        }

        void IEWrapper.PositionMultiEnd(int reqId)
        {
            var tmp = PositionMultiEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.AccountUpdateMulti(int reqId,
                                          string account,
                                          string modelCode,
                                          string key,
                                          string value,
                                          string currency)
        {
            var tmp = AccountUpdateMulti;

            if (tmp != null)
            {
                sc.Post(t => tmp(new AccountUpdateMultiMessage(reqId, account, modelCode, key, value, currency)), null);
            }
        }

        void IEWrapper.AccountUpdateMultiEnd(int reqId)
        {
            var tmp = AccountUpdateMultiEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.SecurityDefinitionOptionParameter(int reqId,
                                                         string exchange,
                                                         int underlyingConId,
                                                         string tradingClass,
                                                         string multiplier,
                                                         HashSet<string> expirations,
                                                         HashSet<double> strikes)
        {
            var tmp = SecurityDefinitionOptionParameter;

            if (tmp != null)
            {
                sc.Post(t => tmp(new SecurityDefinitionOptionParameterMessage(
                                     reqId,
                                     exchange,
                                     underlyingConId,
                                     tradingClass,
                                     multiplier,
                                     expirations,
                                     strikes)),
                        null);
            }
        }

        void IEWrapper.SecurityDefinitionOptionParameterEnd(int reqId)
        {
            var tmp = SecurityDefinitionOptionParameterEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId), null);
            }
        }

        void IEWrapper.SoftDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            var tmp = SoftDollarTiers;

            if (tmp != null)
            {
                sc.Post(t => tmp(new SoftDollarTiersMessage(reqId, tiers)), null);
            }
        }

        void IEWrapper.FamilyCodes(FamilyCode[] familyCodes)
        {
            var tmp = FamilyCodes;

            if (tmp != null)
            {
                sc.Post(t => tmp(familyCodes), null);
            }
        }

        void IEWrapper.SymbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            var tmp = SymbolSamples;

            if (tmp != null)
            {
                sc.Post(t => tmp(new SymbolSamplesMessage(reqId, contractDescriptions)), null);
            }
        }

        void IEWrapper.MktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            var tmp = MktDepthExchanges;

            if (tmp != null)
            {
                sc.Post(t => tmp(depthMktDataDescriptions), null);
            }
        }

        void IEWrapper.TickNews(int tickerId,
                                long timeStamp,
                                string providerCode,
                                string articleId,
                                string headline,
                                string extraData)
        {
            var tmp = TickNews;

            if (tmp != null)
            {
                sc.Post(
                    t => tmp(new TickNewsMessage(tickerId, timeStamp, providerCode, articleId, headline, extraData)),
                    null);
            }
        }

        void IEWrapper.SmartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            var tmp = SmartComponents;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, theMap), null);
            }
        }

        void IEWrapper.TickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            var tmp = TickReqParams;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickReqParamsMessage(tickerId, minTick, bboExchange, snapshotPermissions)), null);
            }
        }

        void IEWrapper.NewsProviders(NewsProvider[] newsProviders)
        {
            var tmp = NewsProviders;

            if (tmp != null)
            {
                sc.Post(t => tmp(newsProviders), null);
            }
        }

        void IEWrapper.NewsArticle(int requestId, int articleType, string articleText)
        {
            var tmp = NewsArticle;

            if (tmp != null)
            {
                sc.Post(t => tmp(new NewsArticleMessage(requestId, articleType, articleText)), null);
            }
        }

        void IEWrapper.HistoricalNews(int requestId,
                                      string time,
                                      string providerCode,
                                      string articleId,
                                      string headline)
        {
            var tmp = HistoricalNews;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistoricalNewsMessage(requestId, time, providerCode, articleId, headline)), null);
            }
        }

        void IEWrapper.HistoricalNewsEnd(int requestId, bool hasMore)
        {
            var tmp = HistoricalNewsEnd;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistoricalNewsEndMessage(requestId, hasMore)), null);
            }
        }

        void IEWrapper.HeadTimestamp(int reqId, string headTimestamp)
        {
            var tmp = HeadTimestamp;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HeadTimestampMessage(reqId, headTimestamp)), null);
            }
        }

        void IEWrapper.HistogramData(int reqId, HistogramEntry[] data)
        {
            var tmp = HistogramData;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistogramDataMessage(reqId, data)), null);
            }
        }

        void IEWrapper.HistoricalDataUpdate(int reqId, Bar bar)
        {
            var tmp = HistoricalDataUpdate;

            if (tmp != null)
            {
                sc.Post(t => tmp(new HistoricalDataMessage(reqId, bar)), null);
            }
        }

        void IEWrapper.RerouteMktDataReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDataReq;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, conId, exchange), null);
            }
        }

        void IEWrapper.RerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDepthReq;

            if (tmp != null)
            {
                sc.Post(t => tmp(reqId, conId, exchange), null);
            }
        }

        void IEWrapper.MarketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            var tmp = MarketRule;

            if (tmp != null)
            {
                sc.Post(t => tmp(new MarketRuleMessage(marketRuleId, priceIncrements)), null);
            }
        }

        void IEWrapper.Pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            var tmp = pnl;

            if (tmp != null)
            {
                sc.Post(t => tmp(new PnLMessage(reqId, dailyPnL, unrealizedPnL, realizedPnL)), null);
            }
        }

        void IEWrapper.PnlSingle(int reqId,
                                 int pos,
                                 double dailyPnL,
                                 double unrealizedPnL,
                                 double realizedPnL,
                                 double value)
        {
            var tmp = pnlSingle;

            if (tmp != null)
            {
                sc.Post(t => tmp(new PnLSingleMessage(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value)), null);
            }
        }

        void IEWrapper.HistoricalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            var tmp = historicalTick;

            if (tmp != null)
            {
                ticks.ToList()
                    .ForEach(tick => sc.Post(
                                 t => tmp(new HistoricalTickMessage(reqId, tick.Time, tick.Price, tick.Size)),
                                 null));
            }
        }

        void IEWrapper.HistoricalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            var tmp = historicalTickBidAsk;

            if (tmp != null)
            {
                ticks.ToList()
                    .ForEach(tick => sc.Post(t =>
                                                 tmp(new HistoricalTickBidAskMessage(
                                                         reqId,
                                                         tick.Time,
                                                         tick.Mask,
                                                         tick.PriceBid,
                                                         tick.PriceAsk,
                                                         tick.SizeBid,
                                                         tick.SizeAsk)),
                                             null));
            }
        }

        void IEWrapper.HistoricalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            var tmp = historicalTickLast;

            if (tmp != null)
            {
                ticks.ToList()
                    .ForEach(tick => sc.Post(t =>
                                                 tmp(new HistoricalTickLastMessage(
                                                         reqId,
                                                         tick.Time,
                                                         tick.Mask,
                                                         tick.Price,
                                                         tick.Size,
                                                         tick.Exchange,
                                                         tick.SpecialConditions)),
                                             null));
            }
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
            var tmp = tickByTickAllLast;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickByTickAllLastMessage(reqId,
                                                              tickType,
                                                              time,
                                                              price,
                                                              size,
                                                              attribs,
                                                              exchange,
                                                              specialConditions)),
                        null);
            }
        }

        void IEWrapper.TickByTickBidAsk(int reqId,
                                        long time,
                                        double bidPrice,
                                        double askPrice,
                                        int bidSize,
                                        int askSize,
                                        TickAttrib attribs)
        {
            var tmp = tickByTickBidAsk;

            if (tmp != null)
            {
                sc.Post(t => tmp(
                            new TickByTickBidAskMessage(reqId, time, bidPrice, askPrice, bidSize, askSize, attribs)),
                        null);
            }
        }

        void IEWrapper.TickByTickMidPoint(int reqId, long time, double midPoint)
        {
            var tmp = tickByTickMidPoint;

            if (tmp != null)
            {
                sc.Post(t => tmp(new TickByTickMidPointMessage(reqId, time, midPoint)), null);
            }
        }

        public Task<Contract> ResolveContractAsync(int conId, ExchangeType refExch)
        {
            var reqId = new Random(DateTime.UtcNow.Millisecond).Next();
            var resolveResult = new TaskCompletionSource<Contract>();
            var resolveContractError = new Action<int, int, string, Exception>((id, code, msg, ex) =>
            {
                if (reqId != id)
                {
                    return;
                }

                resolveResult.SetResult(null);
            });

            var resolveContract = new Action<ContractDetailsMessage>(msg =>
            {
                if (msg.RequestId == reqId)
                {
                    resolveResult.SetResult(msg.ContractDetails.Contract);
                }
            });
            var contractDetailsEnd = new Action<int>(id =>
            {
                if (reqId == id && !resolveResult.Task.IsCompleted)
                {
                    resolveResult.SetResult(null);
                }
            });

            var tmpError = Error;
            var tmpContractDetails = ContractDetails;
            var tmpContractDetailsEnd = ContractDetailsEnd;

            Error = resolveContractError;
            ContractDetails = resolveContract;
            ContractDetailsEnd = contractDetailsEnd;

            resolveResult.Task.ContinueWith(t =>
            {
                Error = tmpError;
                ContractDetails = tmpContractDetails;
                ContractDetailsEnd = tmpContractDetailsEnd;
            });

            ClientSocket.ReqContractDetails(reqId, new Contract { ConId = conId, Exchange = refExch });
            return resolveResult.Task;
        }

        public Task<Contract[]> ResolveContractAsync(SecType secType, string symbol, string currency, ExchangeType exchange)
        {
            var reqId = new Random(DateTime.UtcNow.Millisecond).Next();
            var res = new TaskCompletionSource<Contract[]>();
            var contractList = new List<Contract>();
            var resolveContract_Error = new Action<int, int, string, Exception>((id, code, msg, ex) =>
            {
                if (reqId != id)
                {
                    return;
                }

                res.SetResult(new Contract[0]);
            });
            var contractDetails = new Action<ContractDetailsMessage>(msg =>
            {
                if (reqId != msg.RequestId)
                {
                    return;
                }

                contractList.Add(msg.ContractDetails.Contract);
            });
            var contractDetailsEnd = new Action<int>(id =>
            {
                if (reqId == id)
                {
                    res.SetResult(contractList.ToArray());
                }
            });

            var tmpError = Error;
            var tmpContractDetails = ContractDetails;
            var tmpContractDetailsEnd = ContractDetailsEnd;

            Error = resolveContract_Error;
            ContractDetails = contractDetails;
            ContractDetailsEnd = contractDetailsEnd;

            res.Task.ContinueWith(t =>
            {
                Error = tmpError;
                ContractDetails = tmpContractDetails;
                ContractDetailsEnd = tmpContractDetailsEnd;
            });

            ClientSocket.ReqContractDetails(reqId,
                                            new Contract
                                            {
                                                SecType = secType,
                                                Symbol = symbol,
                                                Currency = currency,
                                                Exchange = exchange
                                            });

            return res.Task;
        }

        public event Action<int, int, string, Exception> Error;

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

        public event Action<NewsProvider[]> NewsProviders;

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