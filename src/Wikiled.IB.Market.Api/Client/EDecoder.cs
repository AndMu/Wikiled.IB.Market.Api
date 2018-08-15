using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Wikiled.IB.Market.Api.Client
{
    internal class EDecoder : IDecoder
    {
        private BinaryReader dataReader;
        private readonly IEClientMsgSink eClientMsgSink;
        private readonly IEWrapper eWrapper;
        private int nDecodedLen;
        private int serverVersion;

        public EDecoder(int serverVersion, IEWrapper callback, IEClientMsgSink sink = null)
        {
            this.serverVersion = serverVersion;
            eWrapper = callback;
            eClientMsgSink = sink;
        }

        public double ReadDouble()
        {
            var doubleAsstring = ReadString();
            if (string.IsNullOrEmpty(doubleAsstring) ||
                doubleAsstring == "0")
            {
                return 0;
            }

            return double.Parse(doubleAsstring, NumberFormatInfo.InvariantInfo);
        }

        public double ReadDoubleMax()
        {
            var str = ReadString();
            return str == null || str.Length == 0 ? double.MaxValue : double.Parse(str, NumberFormatInfo.InvariantInfo);
        }

        public long ReadLong()
        {
            var longAsstring = ReadString();
            if (string.IsNullOrEmpty(longAsstring) ||
                longAsstring == "0")
            {
                return 0;
            }

            return long.Parse(longAsstring);
        }

        public int ReadInt()
        {
            var intAsstring = ReadString();
            if (string.IsNullOrEmpty(intAsstring) ||
                intAsstring == "0")
            {
                return 0;
            }

            return int.Parse(intAsstring);
        }

        public int ReadIntMax()
        {
            var str = ReadString();
            return str == null || str.Length == 0 ? int.MaxValue : int.Parse(str);
        }

        public bool ReadBoolFromInt()
        {
            var str = ReadString();
            return str == null ? false : int.Parse(str) != 0;
        }

        public string ReadString()
        {
            var b = dataReader.ReadByte();

            nDecodedLen++;

            if (b == 0)
            {
                return null;
            }

            var strBuilder = new StringBuilder();
            strBuilder.Append((char)b);
            while (true)
            {
                b = dataReader.ReadByte();
                if (b == 0)
                {
                    break;
                }

                strBuilder.Append((char)b);
            }

            nDecodedLen += strBuilder.Length;

            return strBuilder.ToString();
        }

        public int ParseAndProcessMsg(byte[] buf)
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
            }

            dataReader = new BinaryReader(new MemoryStream(buf));
            nDecodedLen = 0;

            if (serverVersion == 0)
            {
                ProcessConnectAck();

                return nDecodedLen;
            }

            return ProcessIncomingMessage(ReadInt()) ? nDecodedLen : -1;
        }

        private void ProcessConnectAck()
        {
            serverVersion = ReadInt();

            if (serverVersion == -1)
            {
                var srv = ReadString();

                serverVersion = 0;

                if (eClientMsgSink != null)
                {
                    eClientMsgSink.Redirect(srv);
                }

                return;
            }

            var serverTime = "";

            if (serverVersion >= 20)
            {
                serverTime = ReadString();
            }

            if (eClientMsgSink != null)
            {
                eClientMsgSink.ServerVersion(serverVersion, serverTime);
            }

            eWrapper.ConnectAck();
        }

        private bool ProcessIncomingMessage(int incomingMessage)
        {
            if (incomingMessage == IncomingMessage.NotValid)
            {
                return false;
            }

            switch (incomingMessage)
            {
                case IncomingMessage.TickPrice:
                    TickPriceEvent();
                    break;

                case IncomingMessage.TickSize:
                    TickSizeEvent();
                    break;

                case IncomingMessage.Tickstring:
                    TickStringEvent();
                    break;

                case IncomingMessage.TickGeneric:
                    TickGenericEvent();
                    break;

                case IncomingMessage.TickEfp:
                    TickEfpEvent();
                    break;

                case IncomingMessage.TickSnapshotEnd:
                    TickSnapshotEndEvent();
                    break;

                case IncomingMessage.Error:
                    ErrorEvent();
                    break;

                case IncomingMessage.CurrentTime:
                    CurrentTimeEvent();
                    break;

                case IncomingMessage.ManagedAccounts:
                    ManagedAccountsEvent();
                    break;

                case IncomingMessage.NextValidId:
                    NextValidIdEvent();
                    break;

                case IncomingMessage.DeltaNeutralValidation:
                    DeltaNeutralValidationEvent();
                    break;

                case IncomingMessage.TickOptionComputation:
                    TickOptionComputationEvent();
                    break;

                case IncomingMessage.AccountSummary:
                    AccountSummaryEvent();
                    break;

                case IncomingMessage.AccountSummaryEnd:
                    AccountSummaryEndEvent();
                    break;

                case IncomingMessage.AccountValue:
                    AccountValueEvent();
                    break;

                case IncomingMessage.PortfolioValue:
                    PortfolioValueEvent();
                    break;

                case IncomingMessage.AccountUpdateTime:
                    AccountUpdateTimeEvent();
                    break;

                case IncomingMessage.AccountDownloadEnd:
                    AccountDownloadEndEvent();
                    break;

                case IncomingMessage.OrderStatus:
                    OrderStatusEvent();
                    break;

                case IncomingMessage.OpenOrder:
                    OpenOrderEvent();
                    break;

                case IncomingMessage.OpenOrderEnd:
                    OpenOrderEndEvent();
                    break;

                case IncomingMessage.ContractData:
                    ContractDataEvent();
                    break;

                case IncomingMessage.ContractDataEnd:
                    ContractDataEndEvent();
                    break;

                case IncomingMessage.ExecutionData:
                    ExecutionDataEvent();
                    break;

                case IncomingMessage.ExecutionDataEnd:
                    ExecutionDataEndEvent();
                    break;

                case IncomingMessage.CommissionsReport:
                    CommissionReportEvent();
                    break;

                case IncomingMessage.FundamentalData:
                    FundamentalDataEvent();
                    break;

                case IncomingMessage.HistoricalData:
                    HistoricalDataEvent();
                    break;

                case IncomingMessage.MarketDataType:
                    MarketDataTypeEvent();
                    break;

                case IncomingMessage.MarketDepth:
                    MarketDepthEvent();
                    break;

                case IncomingMessage.MarketDepthL2:
                    MarketDepthL2Event();
                    break;

                case IncomingMessage.NewsBulletins:
                    NewsBulletinsEvent();
                    break;

                case IncomingMessage.Position:
                    PositionEvent();
                    break;

                case IncomingMessage.PositionEnd:
                    PositionEndEvent();
                    break;

                case IncomingMessage.RealTimeBars:
                    RealTimeBarsEvent();
                    break;

                case IncomingMessage.ScannerParameters:
                    ScannerParametersEvent();
                    break;

                case IncomingMessage.ScannerData:
                    ScannerDataEvent();
                    break;

                case IncomingMessage.ReceiveFa:
                    ReceiveFaEvent();
                    break;

                case IncomingMessage.BondContractData:
                    BondContractDetailsEvent();
                    break;

                case IncomingMessage.VerifyMessageApi:
                    VerifyMessageApiEvent();
                    break;

                case IncomingMessage.VerifyCompleted:
                    VerifyCompletedEvent();
                    break;

                case IncomingMessage.DisplayGroupList:
                    DisplayGroupListEvent();
                    break;

                case IncomingMessage.DisplayGroupUpdated:
                    DisplayGroupUpdatedEvent();
                    break;

                case IncomingMessage.VerifyAndAuthMessageApi:
                    VerifyAndAuthMessageApiEvent();
                    break;

                case IncomingMessage.VerifyAndAuthCompleted:
                    VerifyAndAuthCompletedEvent();
                    break;

                case IncomingMessage.PositionMulti:
                    PositionMultiEvent();
                    break;

                case IncomingMessage.PositionMultiEnd:
                    PositionMultiEndEvent();
                    break;

                case IncomingMessage.AccountUpdateMulti:
                    AccountUpdateMultiEvent();
                    break;

                case IncomingMessage.AccountUpdateMultiEnd:
                    AccountUpdateMultiEndEvent();
                    break;

                case IncomingMessage.SecurityDefinitionOptionParameter:
                    SecurityDefinitionOptionParameterEvent();
                    break;

                case IncomingMessage.SecurityDefinitionOptionParameterEnd:
                    SecurityDefinitionOptionParameterEndEvent();
                    break;

                case IncomingMessage.SoftDollarTier:
                    SoftDollarTierEvent();
                    break;

                case IncomingMessage.FamilyCodes:
                    FamilyCodesEvent();
                    break;

                case IncomingMessage.SymbolSamples:
                    SymbolSamplesEvent();
                    break;

                case IncomingMessage.MktDepthExchanges:
                    MktDepthExchangesEvent();
                    break;

                case IncomingMessage.TickNews:
                    TickNewsEvent();
                    break;

                case IncomingMessage.TickReqParams:
                    TickReqParamsEvent();
                    break;

                case IncomingMessage.SmartComponents:
                    SmartComponentsEvent();
                    break;

                case IncomingMessage.NewsProviders:
                    NewsProvidersEvent();
                    break;

                case IncomingMessage.NewsArticle:
                    NewsArticleEvent();
                    break;

                case IncomingMessage.HistoricalNews:
                    HistoricalNewsEvent();
                    break;

                case IncomingMessage.HistoricalNewsEnd:
                    HistoricalNewsEndEvent();
                    break;

                case IncomingMessage.HeadTimestamp:
                    HeadTimestampEvent();
                    break;

                case IncomingMessage.HistogramData:
                    HistogramDataEvent();
                    break;

                case IncomingMessage.HistoricalDataUpdate:
                    HistoricalDataUpdateEvent();
                    break;

                case IncomingMessage.RerouteMktDataReq:
                    RerouteMktDataReqEvent();
                    break;

                case IncomingMessage.RerouteMktDepthReq:
                    RerouteMktDepthReqEvent();
                    break;

                case IncomingMessage.MarketRule:
                    MarketRuleEvent();
                    break;

                case IncomingMessage.PnL:
                    PnLEvent();
                    break;

                case IncomingMessage.PnLSingle:
                    PnLSingleEvent();
                    break;

                case IncomingMessage.HistoricalTick:
                    HistoricalTickEvent();
                    break;

                case IncomingMessage.HistoricalTickBidAsk:
                    HistoricalTickBidAskEvent();
                    break;

                case IncomingMessage.HistoricalTickLast:
                    HistoricalTickLastEvent();
                    break;

                case IncomingMessage.TickByTick:
                    TickByTickEvent();
                    break;

                default:
                    eWrapper.Error(IncomingMessage.NotValid,
                                   EClientErrors.UnknownId.Code,
                                   EClientErrors.UnknownId.Message);
                    return false;
            }

            return true;
        }

        private void TickByTickEvent()
        {
            var reqId = ReadInt();
            var tickType = ReadInt();
            var time = ReadLong();
            BitMask mask;
            TickAttrib attribs;

            switch (tickType)
            {
                case 0: // None
                    break;
                case 1: // Last
                case 2: // AllLast
                    var price = ReadDouble();
                    var size = ReadInt();
                    mask = new BitMask(ReadInt());
                    attribs = new TickAttrib();
                    attribs.PastLimit = mask[0];
                    attribs.Unreported = mask[1];
                    var exchange = ReadString();
                    var specialConditions = ReadString();
                    eWrapper.TickByTickAllLast(reqId,
                                               tickType,
                                               time,
                                               price,
                                               size,
                                               attribs,
                                               exchange,
                                               specialConditions);
                    break;
                case 3: // BidAsk
                    var bidPrice = ReadDouble();
                    var askPrice = ReadDouble();
                    var bidSize = ReadInt();
                    var askSize = ReadInt();
                    mask = new BitMask(ReadInt());
                    attribs = new TickAttrib();
                    attribs.BidPastLow = mask[0];
                    attribs.AskPastHigh = mask[1];
                    eWrapper.TickByTickBidAsk(reqId, time, bidPrice, askPrice, bidSize, askSize, attribs);
                    break;
                case 4: // MidPoint
                    var midPoint = ReadDouble();
                    eWrapper.TickByTickMidPoint(reqId, time, midPoint);
                    break;
            }
        }

        private void HistoricalTickLastEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTickLast[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                var mask = ReadInt();
                var price = ReadDouble();
                var size = ReadLong();
                var exchange = ReadString();
                var specialConditions = ReadString();

                ticks[i] = new HistoricalTickLast(time, mask, price, size, exchange, specialConditions);
            }

            var done = ReadBoolFromInt();

            eWrapper.HistoricalTicksLast(reqId, ticks, done);
        }

        private void HistoricalTickBidAskEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTickBidAsk[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                var mask = ReadInt();
                var priceBid = ReadDouble();
                var priceAsk = ReadDouble();
                var sizeBid = ReadLong();
                var sizeAsk = ReadLong();

                ticks[i] = new HistoricalTickBidAsk(time, mask, priceBid, priceAsk, sizeBid, sizeAsk);
            }

            var done = ReadBoolFromInt();

            eWrapper.HistoricalTicksBidAsk(reqId, ticks, done);
        }

        private void HistoricalTickEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTick[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                ReadInt(); // for consistency
                var price = ReadDouble();
                var size = ReadLong();

                ticks[i] = new HistoricalTick(time, price, size);
            }

            var done = ReadBoolFromInt();

            eWrapper.HistoricalTicks(reqId, ticks, done);
        }

        private void MarketRuleEvent()
        {
            var marketRuleId = ReadInt();
            var priceIncrements = new PriceIncrement[0];
            var nPriceIncrements = ReadInt();

            if (nPriceIncrements > 0)
            {
                Array.Resize(ref priceIncrements, nPriceIncrements);

                for (var i = 0; i < nPriceIncrements; ++i)
                {
                    priceIncrements[i] = new PriceIncrement(ReadDouble(), ReadDouble());
                }
            }

            eWrapper.MarketRule(marketRuleId, priceIncrements);
        }

        private void RerouteMktDepthReqEvent()
        {
            var reqId = ReadInt();
            var conId = ReadInt();
            var exchange = ReadString();

            eWrapper.RerouteMktDepthReq(reqId, conId, exchange);
        }

        private void RerouteMktDataReqEvent()
        {
            var reqId = ReadInt();
            var conId = ReadInt();
            var exchange = ReadString();

            eWrapper.RerouteMktDataReq(reqId, conId, exchange);
        }

        private void HistoricalDataUpdateEvent()
        {
            var requestId = ReadInt();
            var barCount = ReadInt();
            var date = ReadString();
            var open = ReadDouble();
            var close = ReadDouble();
            var high = ReadDouble();
            var low = ReadDouble();
            var wap = ReadDouble();
            var volume = ReadLong();

            eWrapper.HistoricalDataUpdate(requestId,
                                          new Bar(date,
                                                  open,
                                                  high,
                                                  low,
                                                  close,
                                                  volume,
                                                  barCount,
                                                  wap));
        }


        private void PnLSingleEvent()
        {
            var reqId = ReadInt();
            var pos = ReadInt();
            var dailyPnL = ReadDouble();
            var unrealizedPnL = double.MaxValue;
            var realizedPnL = double.MaxValue;

            if (serverVersion >= MinServerVer.UnrealizedPnl)
            {
                unrealizedPnL = ReadDouble();
            }

            if (serverVersion >= MinServerVer.RealizedPnl)
            {
                realizedPnL = ReadDouble();
            }

            var value = ReadDouble();

            eWrapper.PnlSingle(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value);
        }

        private void PnLEvent()
        {
            var reqId = ReadInt();
            var dailyPnL = ReadDouble();
            var unrealizedPnL = double.MaxValue;
            var realizedPnL = double.MaxValue;

            if (serverVersion >= MinServerVer.UnrealizedPnl)
            {
                unrealizedPnL = ReadDouble();
            }

            if (serverVersion >= MinServerVer.RealizedPnl)
            {
                realizedPnL = ReadDouble();
            }

            eWrapper.Pnl(reqId, dailyPnL, unrealizedPnL, realizedPnL);
        }

        private void HistogramDataEvent()
        {
            var reqId = ReadInt();
            var n = ReadInt();
            var data = new HistogramEntry[n];

            for (var i = 0; i < n; i++)
            {
                data[i].Price = ReadDouble();
                data[i].Size = ReadLong();
            }

            eWrapper.HistogramData(reqId, data);
        }

        private void HeadTimestampEvent()
        {
            var reqId = ReadInt();
            var headTimestamp = ReadString();

            eWrapper.HeadTimestamp(reqId, headTimestamp);
        }

        private void HistoricalNewsEvent()
        {
            var requestId = ReadInt();
            var time = ReadString();
            var providerCode = ReadString();
            var articleId = ReadString();
            var headline = ReadString();

            eWrapper.HistoricalNews(requestId, time, providerCode, articleId, headline);
        }

        private void HistoricalNewsEndEvent()
        {
            var requestId = ReadInt();
            var hasMore = ReadBoolFromInt();

            eWrapper.HistoricalNewsEnd(requestId, hasMore);
        }

        private void NewsArticleEvent()
        {
            var requestId = ReadInt();
            var articleType = ReadInt();
            var articleText = ReadString();

            eWrapper.NewsArticle(requestId, articleType, articleText);
        }

        private void NewsProvidersEvent()
        {
            var newsProviders = new NewsProvider[0];
            var nNewsProviders = ReadInt();

            if (nNewsProviders > 0)
            {
                Array.Resize(ref newsProviders, nNewsProviders);

                for (var i = 0; i < nNewsProviders; ++i)
                {
                    newsProviders[i] = new NewsProvider(ReadString(), ReadString());
                }
            }

            eWrapper.NewsProviders(newsProviders);
        }

        private void SmartComponentsEvent()
        {
            var reqId = ReadInt();
            var n = ReadInt();
            var theMap = new Dictionary<int, KeyValuePair<string, char>>();

            for (var i = 0; i < n; i++)
            {
                var bitNumber = ReadInt();
                var exchange = ReadString();
                var exchangeLetter = ReadChar();

                theMap.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }

            eWrapper.SmartComponents(reqId, theMap);
        }

        private void TickReqParamsEvent()
        {
            var tickerId = ReadInt();
            var minTick = ReadDouble();
            var bboExchange = ReadString();
            var snapshotPermissions = ReadInt();

            eWrapper.TickReqParams(tickerId, minTick, bboExchange, snapshotPermissions);
        }

        private void TickNewsEvent()
        {
            var tickerId = ReadInt();
            var timeStamp = ReadLong();
            var providerCode = ReadString();
            var articleId = ReadString();
            var headline = ReadString();
            var extraData = ReadString();

            eWrapper.TickNews(tickerId, timeStamp, providerCode, articleId, headline, extraData);
        }

        private void SymbolSamplesEvent()
        {
            var reqId = ReadInt();
            var contractDescriptions = new ContractDescription[0];
            var nContractDescriptions = ReadInt();

            if (nContractDescriptions > 0)
            {
                Array.Resize(ref contractDescriptions, nContractDescriptions);

                for (var i = 0; i < nContractDescriptions; ++i)
                {
                    // read contract fields
                    var contract = new Contract();
                    contract.ConId = ReadInt();
                    contract.Symbol = ReadString();
                    contract.SecType = ReadString();
                    contract.PrimaryExch = ReadString();
                    contract.Currency = ReadString();

                    // read derivative sec types list
                    var derivativeSecTypes = new string[0];
                    var nDerivativeSecTypes = ReadInt();
                    if (nDerivativeSecTypes > 0)
                    {
                        Array.Resize(ref derivativeSecTypes, nDerivativeSecTypes);
                        for (var j = 0; j < nDerivativeSecTypes; ++j)
                        {
                            derivativeSecTypes[j] = ReadString();
                        }
                    }

                    var contractDescription = new ContractDescription(contract, derivativeSecTypes);
                    contractDescriptions[i] = contractDescription;
                }
            }

            eWrapper.SymbolSamples(reqId, contractDescriptions);
        }

        private void FamilyCodesEvent()
        {
            var familyCodes = new FamilyCode[0];
            var nFamilyCodes = ReadInt();

            if (nFamilyCodes > 0)
            {
                Array.Resize(ref familyCodes, nFamilyCodes);

                for (var i = 0; i < nFamilyCodes; ++i)
                {
                    familyCodes[i] = new FamilyCode(ReadString(), ReadString());
                }
            }

            eWrapper.FamilyCodes(familyCodes);
        }

        private void MktDepthExchangesEvent()
        {
            var depthMktDataDescriptions = new DepthMktDataDescription[0];
            var nDescriptions = ReadInt();

            if (nDescriptions > 0)
            {
                Array.Resize(ref depthMktDataDescriptions, nDescriptions);

                for (var i = 0; i < nDescriptions; i++)
                {
                    if (serverVersion >= MinServerVer.ServiceDataType)
                    {
                        depthMktDataDescriptions[i] =
                            new DepthMktDataDescription(ReadString(),
                                                        ReadString(),
                                                        ReadString(),
                                                        ReadString(),
                                                        ReadIntMax());
                    }
                    else
                    {
                        depthMktDataDescriptions[i] = new DepthMktDataDescription(
                            ReadString(),
                            ReadString(),
                            "",
                            ReadBoolFromInt() ? "Deep2" : "Deep",
                            int.MaxValue);
                    }
                }
            }

            eWrapper.MktDepthExchanges(depthMktDataDescriptions);
        }

        private void SoftDollarTierEvent()
        {
            var reqId = ReadInt();
            var nTiers = ReadInt();
            var tiers = new SoftDollarTier[nTiers];

            for (var i = 0; i < nTiers; i++)
            {
                tiers[i] = new SoftDollarTier(ReadString(), ReadString(), ReadString());
            }

            eWrapper.SoftDollarTiers(reqId, tiers);
        }

        private void SecurityDefinitionOptionParameterEndEvent()
        {
            var reqId = ReadInt();

            eWrapper.SecurityDefinitionOptionParameterEnd(reqId);
        }

        private void SecurityDefinitionOptionParameterEvent()
        {
            var reqId = ReadInt();
            var exchange = ReadString();
            var underlyingConId = ReadInt();
            var tradingClass = ReadString();
            var multiplier = ReadString();
            var expirationsSize = ReadInt();
            var expirations = new HashSet<string>();
            var strikes = new HashSet<double>();

            for (var i = 0; i < expirationsSize; i++)
            {
                expirations.Add(ReadString());
            }

            var strikesSize = ReadInt();

            for (var i = 0; i < strikesSize; i++)
            {
                strikes.Add(ReadDouble());
            }

            eWrapper.SecurityDefinitionOptionParameter(reqId,
                                                       exchange,
                                                       underlyingConId,
                                                       tradingClass,
                                                       multiplier,
                                                       expirations,
                                                       strikes);
        }

        private void DisplayGroupUpdatedEvent()
        {
            var msgVersion = ReadInt();
            var reqId = ReadInt();
            var contractInfo = ReadString();

            eWrapper.DisplayGroupUpdated(reqId, contractInfo);
        }

        private void DisplayGroupListEvent()
        {
            var msgVersion = ReadInt();
            var reqId = ReadInt();
            var groups = ReadString();

            eWrapper.DisplayGroupList(reqId, groups);
        }

        private void VerifyCompletedEvent()
        {
            var msgVersion = ReadInt();
            var isSuccessful = string.Compare(ReadString(), "true", true) == 0;
            var errorText = ReadString();

            eWrapper.VerifyCompleted(isSuccessful, errorText);
        }

        private void VerifyMessageApiEvent()
        {
            var msgVersion = ReadInt();
            var apiData = ReadString();

            eWrapper.VerifyMessageApi(apiData);
        }

        private void VerifyAndAuthCompletedEvent()
        {
            var msgVersion = ReadInt();
            var isSuccessful = string.Compare(ReadString(), "true", true) == 0;
            var errorText = ReadString();

            eWrapper.VerifyAndAuthCompleted(isSuccessful, errorText);
        }

        private void VerifyAndAuthMessageApiEvent()
        {
            var msgVersion = ReadInt();
            var apiData = ReadString();
            var xyzChallenge = ReadString();

            eWrapper.VerifyAndAuthMessageApi(apiData, xyzChallenge);
        }

        private void TickPriceEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var price = ReadDouble();
            var size = 0;

            if (msgVersion >= 2)
            {
                size = ReadInt();
            }

            var attr = new TickAttrib();

            if (msgVersion >= 3)
            {
                var attrMask = ReadInt();

                attr.CanAutoExecute = attrMask == 1;

                if (serverVersion >= MinServerVer.PastLimit)
                {
                    var mask = new BitMask(attrMask);

                    attr.CanAutoExecute = mask[0];
                    attr.PastLimit = mask[1];

                    if (serverVersion >= MinServerVer.PreOpenBidAsk)
                    {
                        attr.PreOpen = mask[2];
                    }
                }
            }


            eWrapper.TickPrice(requestId, tickType, price, attr);

            if (msgVersion >= 2)
            {
                var sizeTickType = -1; //not a tick
                switch (tickType)
                {
                    case TickType.Bid:
                        sizeTickType = TickType.BidSize;
                        break;
                    case TickType.Ask:
                        sizeTickType = TickType.AskSize;
                        break;
                    case TickType.Last:
                        sizeTickType = TickType.LastSize;
                        break;
                    case TickType.DelayedBid:
                        sizeTickType = TickType.DelayedBidSize;
                        break;
                    case TickType.DelayedAsk:
                        sizeTickType = TickType.DelayedAskSize;
                        break;
                    case TickType.DelayedLast:
                        sizeTickType = TickType.DelayedLastSize;
                        break;
                }

                if (sizeTickType != -1)
                {
                    eWrapper.TickSize(requestId, sizeTickType, size);
                }
            }
        }

        private void TickSizeEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var size = ReadInt();
            eWrapper.TickSize(requestId, tickType, size);
        }

        private void TickStringEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var value = ReadString();
            eWrapper.TickString(requestId, tickType, value);
        }

        private void TickGenericEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var value = ReadDouble();
            eWrapper.TickGeneric(requestId, tickType, value);
        }

        private void TickEfpEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var basisPoints = ReadDouble();
            var formattedBasisPoints = ReadString();
            var impliedFuturesPrice = ReadDouble();
            var holdDays = ReadInt();
            var futureLastTradeDate = ReadString();
            var dividendImpact = ReadDouble();
            var dividendsToLastTradeDate = ReadDouble();
            eWrapper.TickEfp(requestId,
                             tickType,
                             basisPoints,
                             formattedBasisPoints,
                             impliedFuturesPrice,
                             holdDays,
                             futureLastTradeDate,
                             dividendImpact,
                             dividendsToLastTradeDate);
        }

        private void TickSnapshotEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.TickSnapshotEnd(requestId);
        }

        private void ErrorEvent()
        {
            var msgVersion = ReadInt();
            if (msgVersion < 2)
            {
                var msg = ReadString();
                eWrapper.Error(msg);
            }
            else
            {
                var id = ReadInt();
                var errorCode = ReadInt();
                var errorMsg = ReadString();
                eWrapper.Error(id, errorCode, errorMsg);
            }
        }

        private void CurrentTimeEvent()
        {
            var msgVersion = ReadInt(); //version
            var time = ReadLong();
            eWrapper.CurrentTime(time);
        }

        private void ManagedAccountsEvent()
        {
            var msgVersion = ReadInt();
            var accountsList = ReadString();
            eWrapper.ManagedAccounts(accountsList);
        }

        private void NextValidIdEvent()
        {
            var msgVersion = ReadInt();
            var orderId = ReadInt();
            eWrapper.NextValidId(orderId);
        }

        private void DeltaNeutralValidationEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var deltaNeutralContract = new DeltaNeutralContract();
            deltaNeutralContract.ConId = ReadInt();
            deltaNeutralContract.Delta = ReadDouble();
            deltaNeutralContract.Price = ReadDouble();
            eWrapper.DeltaNeutralValidation(requestId, deltaNeutralContract);
        }

        private void TickOptionComputationEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var impliedVolatility = ReadDouble();
            if (impliedVolatility.Equals(-1))
            {
                // -1 is the "not yet computed" indicator
                impliedVolatility = double.MaxValue;
            }

            var delta = ReadDouble();
            if (delta.Equals(-2))
            {
                // -2 is the "not yet computed" indicator
                delta = double.MaxValue;
            }

            var optPrice = double.MaxValue;
            var pvDividend = double.MaxValue;
            var gamma = double.MaxValue;
            var vega = double.MaxValue;
            var theta = double.MaxValue;
            var undPrice = double.MaxValue;
            if (msgVersion >= 6 || tickType == TickType.ModelOption || tickType == TickType.DelayedModelOption)
            {
                optPrice = ReadDouble();
                if (optPrice.Equals(-1))
                {
                    // -1 is the "not yet computed" indicator
                    optPrice = double.MaxValue;
                }

                pvDividend = ReadDouble();
                if (pvDividend.Equals(-1))
                {
                    // -1 is the "not yet computed" indicator
                    pvDividend = double.MaxValue;
                }
            }

            if (msgVersion >= 6)
            {
                gamma = ReadDouble();
                if (gamma.Equals(-2))
                {
                    // -2 is the "not yet computed" indicator
                    gamma = double.MaxValue;
                }

                vega = ReadDouble();
                if (vega.Equals(-2))
                {
                    // -2 is the "not yet computed" indicator
                    vega = double.MaxValue;
                }

                theta = ReadDouble();
                if (theta.Equals(-2))
                {
                    // -2 is the "not yet computed" indicator
                    theta = double.MaxValue;
                }

                undPrice = ReadDouble();
                if (undPrice.Equals(-1))
                {
                    // -1 is the "not yet computed" indicator
                    undPrice = double.MaxValue;
                }
            }

            eWrapper.TickOptionComputation(requestId,
                                           tickType,
                                           impliedVolatility,
                                           delta,
                                           optPrice,
                                           pvDividend,
                                           gamma,
                                           vega,
                                           theta,
                                           undPrice);
        }

        private void AccountSummaryEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var account = ReadString();
            var tag = ReadString();
            var value = ReadString();
            var currency = ReadString();
            eWrapper.AccountSummary(requestId, account, tag, value, currency);
        }

        private void AccountSummaryEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.AccountSummaryEnd(requestId);
        }

        private void AccountValueEvent()
        {
            var msgVersion = ReadInt();
            var key = ReadString();
            var value = ReadString();
            var currency = ReadString();
            string accountName = null;
            if (msgVersion >= 2)
            {
                accountName = ReadString();
            }

            eWrapper.UpdateAccountValue(key, value, currency, accountName);
        }

        private void BondContractDetailsEvent()
        {
            var msgVersion = ReadInt();
            var requestId = -1;
            if (msgVersion >= 3)
            {
                requestId = ReadInt();
            }

            var contract = new ContractDetails();

            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadString();
            contract.Cusip = ReadString();
            contract.Coupon = ReadDouble();
            ReadLastTradeDate(contract, true);
            contract.IssueDate = ReadString();
            contract.Ratings = ReadString();
            contract.BondType = ReadString();
            contract.CouponType = ReadString();
            contract.Convertible = ReadBoolFromInt();
            contract.Callable = ReadBoolFromInt();
            contract.Putable = ReadBoolFromInt();
            contract.DescAppend = ReadString();
            contract.Contract.Exchange = ReadString();
            contract.Contract.Currency = ReadString();
            contract.MarketName = ReadString();
            contract.Contract.TradingClass = ReadString();
            contract.Contract.ConId = ReadInt();
            contract.MinTick = ReadDouble();
            if (serverVersion >= MinServerVer.MdSizeMultiplier)
            {
                contract.MdSizeMultiplier = ReadInt();
            }

            contract.OrderTypes = ReadString();
            contract.ValidExchanges = ReadString();
            if (msgVersion >= 2)
            {
                contract.NextOptionDate = ReadString();
                contract.NextOptionType = ReadString();
                contract.NextOptionPartial = ReadBoolFromInt();
                contract.Notes = ReadString();
            }

            if (msgVersion >= 4)
            {
                contract.LongName = ReadString();
            }

            if (msgVersion >= 6)
            {
                contract.EvRule = ReadString();
                contract.EvMultiplier = ReadDouble();
            }

            if (msgVersion >= 5)
            {
                var secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>();
                    for (var i = 0; i < secIdListCount; ++i)
                    {
                        var tagValue = new TagValue();
                        tagValue.Tag = ReadString();
                        tagValue.Value = ReadString();
                        contract.SecIdList.Add(tagValue);
                    }
                }
            }

            if (serverVersion >= MinServerVer.AggGroup)
            {
                contract.AggGroup = ReadInt();
            }

            if (serverVersion >= MinServerVer.MarketRules)
            {
                contract.MarketRuleIds = ReadString();
            }

            eWrapper.BondContractDetails(requestId, contract);
        }

        private void PortfolioValueEvent()
        {
            var msgVersion = ReadInt();
            var contract = new Contract();
            if (msgVersion >= 6)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 7)
            {
                contract.Multiplier = ReadString();
                contract.PrimaryExch = ReadString();
            }

            contract.Currency = ReadString();
            if (msgVersion >= 2)
            {
                contract.LocalSymbol = ReadString();
            }

            if (msgVersion >= 8)
            {
                contract.TradingClass = ReadString();
            }

            var position = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            var marketPrice = ReadDouble();
            var marketValue = ReadDouble();
            var averageCost = 0.0;
            var unrealizedPnl = 0.0;
            var realizedPnl = 0.0;
            if (msgVersion >= 3)
            {
                averageCost = ReadDouble();
                unrealizedPnl = ReadDouble();
                realizedPnl = ReadDouble();
            }

            string accountName = null;
            if (msgVersion >= 4)
            {
                accountName = ReadString();
            }

            if (msgVersion == 6 && serverVersion == 39)
            {
                contract.PrimaryExch = ReadString();
            }

            eWrapper.UpdatePortfolio(contract,
                                     position,
                                     marketPrice,
                                     marketValue,
                                     averageCost,
                                     unrealizedPnl,
                                     realizedPnl,
                                     accountName);
        }

        private void AccountUpdateTimeEvent()
        {
            var msgVersion = ReadInt();
            var timestamp = ReadString();
            eWrapper.UpdateAccountTime(timestamp);
        }

        private void AccountDownloadEndEvent()
        {
            var msgVersion = ReadInt();
            var account = ReadString();
            eWrapper.AccountDownloadEnd(account);
        }

        private void OrderStatusEvent()
        {
            var msgVersion = serverVersion >= MinServerVer.MarketCapPrice ? int.MaxValue : ReadInt();
            var id = ReadInt();
            var status = ReadString();
            var filled = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            var remaining = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            var avgFillPrice = ReadDouble();

            var permId = 0;
            if (msgVersion >= 2)
            {
                permId = ReadInt();
            }

            var parentId = 0;
            if (msgVersion >= 3)
            {
                parentId = ReadInt();
            }

            double lastFillPrice = 0;
            if (msgVersion >= 4)
            {
                lastFillPrice = ReadDouble();
            }

            var clientId = 0;
            if (msgVersion >= 5)
            {
                clientId = ReadInt();
            }

            string whyHeld = null;
            if (msgVersion >= 6)
            {
                whyHeld = ReadString();
            }

            var mktCapPrice = double.MaxValue;

            if (serverVersion >= MinServerVer.MarketCapPrice)
            {
                mktCapPrice = ReadDouble();
            }

            eWrapper.OrderStatus(id,
                                 status,
                                 filled,
                                 remaining,
                                 avgFillPrice,
                                 permId,
                                 parentId,
                                 lastFillPrice,
                                 clientId,
                                 whyHeld,
                                 mktCapPrice);
        }

        private void OpenOrderEvent()
        {
            var msgVersion = ReadInt();
            // read order id
            var order = new Order();
            order.OrderId = ReadInt();

            // read contract fields
            var contract = new Contract();
            if (msgVersion >= 17)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 32)
            {
                contract.Multiplier = ReadString();
            }

            contract.Exchange = ReadString();
            contract.Currency = ReadString();
            if (msgVersion >= 2)
            {
                contract.LocalSymbol = ReadString();
            }

            if (msgVersion >= 32)
            {
                contract.TradingClass = ReadString();
            }

            // read order fields
            order.Action = ReadString();
            order.TotalQuantity = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            order.OrderType = ReadString();
            if (msgVersion < 29)
            {
                order.LmtPrice = ReadDouble();
            }
            else
            {
                order.LmtPrice = ReadDoubleMax();
            }

            if (msgVersion < 30)
            {
                order.AuxPrice = ReadDouble();
            }
            else
            {
                order.AuxPrice = ReadDoubleMax();
            }

            order.Tif = ReadString();
            order.OcaGroup = ReadString();
            order.Account = ReadString();
            order.OpenClose = ReadString();
            order.Origin = ReadInt();
            order.OrderRef = ReadString();

            if (msgVersion >= 3)
            {
                order.ClientId = ReadInt();
            }

            if (msgVersion >= 4)
            {
                order.PermId = ReadInt();
                if (msgVersion < 18)
                {
                    // will never happen
                    /* order.ignoreRth = */
                    ReadBoolFromInt();
                }
                else
                {
                    order.OutsideRth = ReadBoolFromInt();
                }

                order.Hidden = ReadInt() == 1;
                order.DiscretionaryAmt = ReadDouble();
            }

            if (msgVersion >= 5)
            {
                order.GoodAfterTime = ReadString();
            }

            if (msgVersion >= 6)
            {
                // skip deprecated sharesAllocation field
                ReadString();
            }

            if (msgVersion >= 7)
            {
                order.FaGroup = ReadString();
                order.FaMethod = ReadString();
                order.FaPercentage = ReadString();
                order.FaProfile = ReadString();
            }

            if (serverVersion >= MinServerVer.ModelsSupport)
            {
                order.ModelCode = ReadString();
            }

            if (msgVersion >= 8)
            {
                order.GoodTillDate = ReadString();
            }

            if (msgVersion >= 9)
            {
                order.Rule80A = ReadString();
                order.PercentOffset = ReadDoubleMax();
                order.SettlingFirm = ReadString();
                order.ShortSaleSlot = ReadInt();
                order.DesignatedLocation = ReadString();
                if (serverVersion == 51)
                {
                    ReadInt(); // exemptCode
                }
                else if (msgVersion >= 23)
                {
                    order.ExemptCode = ReadInt();
                }

                order.AuctionStrategy = ReadInt();
                order.StartingPrice = ReadDoubleMax();
                order.StockRefPrice = ReadDoubleMax();
                order.Delta = ReadDoubleMax();
                order.StockRangeLower = ReadDoubleMax();
                order.StockRangeUpper = ReadDoubleMax();
                order.DisplaySize = ReadInt();
                if (msgVersion < 18)
                {
                    // will never happen
                    /* order.rthOnly = */
                    ReadBoolFromInt();
                }

                order.BlockOrder = ReadBoolFromInt();
                order.SweepToFill = ReadBoolFromInt();
                order.AllOrNone = ReadBoolFromInt();
                order.MinQty = ReadIntMax();
                order.OcaType = ReadInt();
                order.ETradeOnly = ReadBoolFromInt();
                order.FirmQuoteOnly = ReadBoolFromInt();
                order.NbboPriceCap = ReadDoubleMax();
            }

            if (msgVersion >= 10)
            {
                order.ParentId = ReadInt();
                order.TriggerMethod = ReadInt();
            }

            if (msgVersion >= 11)
            {
                order.Volatility = ReadDoubleMax();
                order.VolatilityType = ReadInt();
                if (msgVersion == 11)
                {
                    var receivedInt = ReadInt();
                    order.DeltaNeutralOrderType = receivedInt == 0 ? "NONE" : "MKT";
                }
                else
                {
                    // msgVersion 12 and up
                    order.DeltaNeutralOrderType = ReadString();
                    order.DeltaNeutralAuxPrice = ReadDoubleMax();

                    if (msgVersion >= 27 && !Util.StringIsEmpty(order.DeltaNeutralOrderType))
                    {
                        order.DeltaNeutralConId = ReadInt();
                        order.DeltaNeutralSettlingFirm = ReadString();
                        order.DeltaNeutralClearingAccount = ReadString();
                        order.DeltaNeutralClearingIntent = ReadString();
                    }

                    if (msgVersion >= 31 && !Util.StringIsEmpty(order.DeltaNeutralOrderType))
                    {
                        order.DeltaNeutralOpenClose = ReadString();
                        order.DeltaNeutralShortSale = ReadBoolFromInt();
                        order.DeltaNeutralShortSaleSlot = ReadInt();
                        order.DeltaNeutralDesignatedLocation = ReadString();
                    }
                }

                order.ContinuousUpdate = ReadInt();
                if (serverVersion == 26)
                {
                    order.StockRangeLower = ReadDouble();
                    order.StockRangeUpper = ReadDouble();
                }

                order.ReferencePriceType = ReadInt();
            }

            if (msgVersion >= 13)
            {
                order.TrailStopPrice = ReadDoubleMax();
            }

            if (msgVersion >= 30)
            {
                order.TrailingPercent = ReadDoubleMax();
            }

            if (msgVersion >= 14)
            {
                order.BasisPoints = ReadDoubleMax();
                order.BasisPointsType = ReadIntMax();
                contract.ComboLegsDescription = ReadString();
            }

            if (msgVersion >= 29)
            {
                var comboLegsCount = ReadInt();
                if (comboLegsCount > 0)
                {
                    contract.ComboLegs = new List<ComboLeg>(comboLegsCount);
                    for (var i = 0; i < comboLegsCount; ++i)
                    {
                        var conId = ReadInt();
                        var ratio = ReadInt();
                        var action = ReadString();
                        var exchange = ReadString();
                        var openClose = ReadInt();
                        var shortSaleSlot = ReadInt();
                        var designatedLocation = ReadString();
                        var exemptCode = ReadInt();

                        var comboLeg = new ComboLeg(conId,
                                                    ratio,
                                                    action,
                                                    exchange,
                                                    openClose,
                                                    shortSaleSlot,
                                                    designatedLocation,
                                                    exemptCode);
                        contract.ComboLegs.Add(comboLeg);
                    }
                }

                var orderComboLegsCount = ReadInt();
                if (orderComboLegsCount > 0)
                {
                    order.OrderComboLegs = new List<OrderComboLeg>(orderComboLegsCount);
                    for (var i = 0; i < orderComboLegsCount; ++i)
                    {
                        var price = ReadDoubleMax();

                        var orderComboLeg = new OrderComboLeg(price);
                        order.OrderComboLegs.Add(orderComboLeg);
                    }
                }
            }

            if (msgVersion >= 26)
            {
                var smartComboRoutingParamsCount = ReadInt();
                if (smartComboRoutingParamsCount > 0)
                {
                    order.SmartComboRoutingParams = new List<TagValue>(smartComboRoutingParamsCount);
                    for (var i = 0; i < smartComboRoutingParamsCount; ++i)
                    {
                        var tagValue = new TagValue();
                        tagValue.Tag = ReadString();
                        tagValue.Value = ReadString();
                        order.SmartComboRoutingParams.Add(tagValue);
                    }
                }
            }

            if (msgVersion >= 15)
            {
                if (msgVersion >= 20)
                {
                    order.ScaleInitLevelSize = ReadIntMax();
                    order.ScaleSubsLevelSize = ReadIntMax();
                }
                else
                {
                    /* int notSuppScaleNumComponents = */
                    ReadIntMax();
                    order.ScaleInitLevelSize = ReadIntMax();
                }

                order.ScalePriceIncrement = ReadDoubleMax();
            }

            if (msgVersion >= 28 && order.ScalePriceIncrement > 0.0 && order.ScalePriceIncrement != double.MaxValue)
            {
                order.ScalePriceAdjustValue = ReadDoubleMax();
                order.ScalePriceAdjustInterval = ReadIntMax();
                order.ScaleProfitOffset = ReadDoubleMax();
                order.ScaleAutoReset = ReadBoolFromInt();
                order.ScaleInitPosition = ReadIntMax();
                order.ScaleInitFillQty = ReadIntMax();
                order.ScaleRandomPercent = ReadBoolFromInt();
            }

            if (msgVersion >= 24)
            {
                order.HedgeType = ReadString();
                if (!Util.StringIsEmpty(order.HedgeType))
                {
                    order.HedgeParam = ReadString();
                }
            }

            if (msgVersion >= 25)
            {
                order.OptOutSmartRouting = ReadBoolFromInt();
            }

            if (msgVersion >= 19)
            {
                order.ClearingAccount = ReadString();
                order.ClearingIntent = ReadString();
            }

            if (msgVersion >= 22)
            {
                order.NotHeld = ReadBoolFromInt();
            }

            if (msgVersion >= 20)
            {
                if (ReadBoolFromInt())
                {
                    var deltaNeutralContract = new DeltaNeutralContract();
                    deltaNeutralContract.ConId = ReadInt();
                    deltaNeutralContract.Delta = ReadDouble();
                    deltaNeutralContract.Price = ReadDouble();
                    contract.DeltaNeutralContract = deltaNeutralContract;
                }
            }

            if (msgVersion >= 21)
            {
                order.AlgoStrategy = ReadString();
                if (!Util.StringIsEmpty(order.AlgoStrategy))
                {
                    var algoParamsCount = ReadInt();
                    if (algoParamsCount > 0)
                    {
                        order.AlgoParams = new List<TagValue>(algoParamsCount);
                        for (var i = 0; i < algoParamsCount; ++i)
                        {
                            var tagValue = new TagValue();
                            tagValue.Tag = ReadString();
                            tagValue.Value = ReadString();
                            order.AlgoParams.Add(tagValue);
                        }
                    }
                }
            }

            if (msgVersion >= 33)
            {
                order.Solicited = ReadBoolFromInt();
            }

            var orderState = new OrderState();
            if (msgVersion >= 16)
            {
                order.WhatIf = ReadBoolFromInt();
                orderState.Status = ReadString();
                if (serverVersion >= MinServerVer.WhatIfExtFields)
                {
                    orderState.InitMarginBefore = ReadString();
                    orderState.MaintMarginBefore = ReadString();
                    orderState.EquityWithLoanBefore = ReadString();
                    orderState.InitMarginChange = ReadString();
                    orderState.MaintMarginChange = ReadString();
                    orderState.EquityWithLoanChange = ReadString();
                }

                orderState.InitMarginAfter = ReadString();
                orderState.MaintMarginAfter = ReadString();
                orderState.EquityWithLoanAfter = ReadString();
                orderState.Commission = ReadDoubleMax();
                orderState.MinCommission = ReadDoubleMax();
                orderState.MaxCommission = ReadDoubleMax();
                orderState.CommissionCurrency = ReadString();
                orderState.WarningText = ReadString();
            }

            if (msgVersion >= 34)
            {
                order.RandomizeSize = ReadBoolFromInt();
                order.RandomizePrice = ReadBoolFromInt();
            }

            if (serverVersion >= MinServerVer.PeggedToBenchmark)
            {
                if (order.OrderType == "PEG BENCH")
                {
                    order.ReferenceContractId = ReadInt();
                    order.IsPeggedChangeAmountDecrease = ReadBoolFromInt();
                    order.PeggedChangeAmount = ReadDoubleMax();
                    order.ReferenceChangeAmount = ReadDoubleMax();
                    order.ReferenceExchange = ReadString();
                }

                var nConditions = ReadInt();

                if (nConditions > 0)
                {
                    for (var i = 0; i < nConditions; i++)
                    {
                        var orderConditionType = (OrderConditionType)ReadInt();
                        var condition = OrderCondition.Create(orderConditionType);

                        condition.Deserialize(this);
                        order.Conditions.Add(condition);
                    }

                    order.ConditionsIgnoreRth = ReadBoolFromInt();
                    order.ConditionsCancelOrder = ReadBoolFromInt();
                }

                order.AdjustedOrderType = ReadString();
                order.TriggerPrice = ReadDoubleMax();
                order.TrailStopPrice = ReadDoubleMax();
                order.LmtPriceOffset = ReadDoubleMax();
                order.AdjustedStopPrice = ReadDoubleMax();
                order.AdjustedStopLimitPrice = ReadDoubleMax();
                order.AdjustedTrailingAmount = ReadDoubleMax();
                order.AdjustableTrailingUnit = ReadInt();
            }

            if (serverVersion >= MinServerVer.SoftDollarTier)
            {
                order.Tier = new SoftDollarTier(ReadString(), ReadString(), ReadString());
            }

            if (serverVersion >= MinServerVer.CashQty)
            {
                order.CashQty = ReadDoubleMax();
            }

            if (serverVersion >= MinServerVer.AutoPriceForHedge)
            {
                order.DontUseAutoPriceForHedge = ReadBoolFromInt();
            }

            eWrapper.OpenOrder(order.OrderId, contract, order, orderState);
        }

        private void OpenOrderEndEvent()
        {
            var msgVersion = ReadInt();
            eWrapper.OpenOrderEnd();
        }

        private void ContractDataEvent()
        {
            var msgVersion = ReadInt();
            var requestId = -1;
            if (msgVersion >= 3)
            {
                requestId = ReadInt();
            }

            var contract = new ContractDetails();
            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadString();
            ReadLastTradeDate(contract, false);
            contract.Contract.Strike = ReadDouble();
            contract.Contract.Right = ReadString();
            contract.Contract.Exchange = ReadString();
            contract.Contract.Currency = ReadString();
            contract.Contract.LocalSymbol = ReadString();
            contract.MarketName = ReadString();
            contract.Contract.TradingClass = ReadString();
            contract.Contract.ConId = ReadInt();
            contract.MinTick = ReadDouble();
            if (serverVersion >= MinServerVer.MdSizeMultiplier)
            {
                contract.MdSizeMultiplier = ReadInt();
            }

            contract.Contract.Multiplier = ReadString();
            contract.OrderTypes = ReadString();
            contract.ValidExchanges = ReadString();
            if (msgVersion >= 2)
            {
                contract.PriceMagnifier = ReadInt();
            }

            if (msgVersion >= 4)
            {
                contract.UnderConId = ReadInt();
            }

            if (msgVersion >= 5)
            {
                contract.LongName = ReadString();
                contract.Contract.PrimaryExch = ReadString();
            }

            if (msgVersion >= 6)
            {
                contract.ContractMonth = ReadString();
                contract.Industry = ReadString();
                contract.Category = ReadString();
                contract.Subcategory = ReadString();
                contract.TimeZoneId = ReadString();
                contract.TradingHours = ReadString();
                contract.LiquidHours = ReadString();
            }

            if (msgVersion >= 8)
            {
                contract.EvRule = ReadString();
                contract.EvMultiplier = ReadDouble();
            }

            if (msgVersion >= 7)
            {
                var secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>(secIdListCount);
                    for (var i = 0; i < secIdListCount; ++i)
                    {
                        var tagValue = new TagValue();
                        tagValue.Tag = ReadString();
                        tagValue.Value = ReadString();
                        contract.SecIdList.Add(tagValue);
                    }
                }
            }

            if (serverVersion >= MinServerVer.AggGroup)
            {
                contract.AggGroup = ReadInt();
            }

            if (serverVersion >= MinServerVer.UnderlyingInfo)
            {
                contract.UnderSymbol = ReadString();
                contract.UnderSecType = ReadString();
            }

            if (serverVersion >= MinServerVer.MarketRules)
            {
                contract.MarketRuleIds = ReadString();
            }

            if (serverVersion >= MinServerVer.RealExpirationDate)
            {
                contract.RealExpirationDate = ReadString();
            }

            eWrapper.ContractDetails(requestId, contract);
        }


        private void ContractDataEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.ContractDetailsEnd(requestId);
        }

        private void ExecutionDataEvent()
        {
            var msgVersion = serverVersion;

            if (serverVersion < MinServerVer.LastLiquidity)
            {
                msgVersion = ReadInt();
            }

            var requestId = -1;
            if (msgVersion >= 7)
            {
                requestId = ReadInt();
            }

            var orderId = ReadInt();
            var contract = new Contract();
            if (msgVersion >= 5)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 9)
            {
                contract.Multiplier = ReadString();
            }

            contract.Exchange = ReadString();
            contract.Currency = ReadString();
            contract.LocalSymbol = ReadString();
            if (msgVersion >= 10)
            {
                contract.TradingClass = ReadString();
            }

            var exec = new Execution();
            exec.OrderId = orderId;
            exec.ExecId = ReadString();
            exec.Time = ReadString();
            exec.AcctNumber = ReadString();
            exec.Exchange = ReadString();
            exec.Side = ReadString();
            exec.Shares = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            exec.Price = ReadDouble();
            if (msgVersion >= 2)
            {
                exec.PermId = ReadInt();
            }

            if (msgVersion >= 3)
            {
                exec.ClientId = ReadInt();
            }

            if (msgVersion >= 4)
            {
                exec.Liquidation = ReadInt();
            }

            if (msgVersion >= 6)
            {
                exec.CumQty = ReadDouble();
                exec.AvgPrice = ReadDouble();
            }

            if (msgVersion >= 8)
            {
                exec.OrderRef = ReadString();
            }

            if (msgVersion >= 9)
            {
                exec.EvRule = ReadString();
                exec.EvMultiplier = ReadDouble();
            }

            if (serverVersion >= MinServerVer.ModelsSupport)
            {
                exec.ModelCode = ReadString();
            }

            if (serverVersion >= MinServerVer.LastLiquidity)
            {
                exec.LastLiquidity = new Liquidity(ReadInt());
            }

            eWrapper.ExecDetails(requestId, contract, exec);
        }

        private void ExecutionDataEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.ExecDetailsEnd(requestId);
        }

        private void CommissionReportEvent()
        {
            var msgVersion = ReadInt();
            var commissionReport = new CommissionReport();
            commissionReport.ExecId = ReadString();
            commissionReport.Commission = ReadDouble();
            commissionReport.Currency = ReadString();
            commissionReport.RealizedPnl = ReadDouble();
            commissionReport.Yield = ReadDouble();
            commissionReport.YieldRedemptionDate = ReadInt();
            eWrapper.CommissionReport(commissionReport);
        }

        private void FundamentalDataEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var fundamentalData = ReadString();
            eWrapper.FundamentalData(requestId, fundamentalData);
        }

        private void HistoricalDataEvent()
        {
            var msgVersion = int.MaxValue;

            if (serverVersion < MinServerVer.SyntRealtimeBars)
            {
                msgVersion = ReadInt();
            }

            var requestId = ReadInt();
            var startDateStr = "";
            var endDateStr = "";

            if (msgVersion >= 2)
            {
                startDateStr = ReadString();
                endDateStr = ReadString();
            }

            var itemCount = ReadInt();

            for (var ctr = 0; ctr < itemCount; ctr++)
            {
                var date = ReadString();
                var open = ReadDouble();
                var high = ReadDouble();
                var low = ReadDouble();
                var close = ReadDouble();
                var volume = serverVersion < MinServerVer.SyntRealtimeBars ? ReadInt() : ReadLong();
                var wap = ReadDouble();

                if (serverVersion < MinServerVer.SyntRealtimeBars)
                {
                    /*string hasGaps = */
                    ReadString();
                }

                var barCount = -1;

                if (msgVersion >= 3)
                {
                    barCount = ReadInt();
                }

                eWrapper.HistoricalData(requestId,
                                        new Bar(date,
                                                open,
                                                high,
                                                low,
                                                close,
                                                volume,
                                                barCount,
                                                wap));
            }

            // send end of dataset marker.
            eWrapper.HistoricalDataEnd(requestId, startDateStr, endDateStr);
        }

        private void MarketDataTypeEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var marketDataType = ReadInt();
            eWrapper.MarketDataType(requestId, marketDataType);
        }

        private void MarketDepthEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var position = ReadInt();
            var operation = ReadInt();
            var side = ReadInt();
            var price = ReadDouble();
            var size = ReadInt();
            eWrapper.UpdateMktDepth(requestId, position, operation, side, price, size);
        }

        private void MarketDepthL2Event()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var position = ReadInt();
            var marketMaker = ReadString();
            var operation = ReadInt();
            var side = ReadInt();
            var price = ReadDouble();
            var size = ReadInt();
            eWrapper.UpdateMktDepthL2(requestId, position, marketMaker, operation, side, price, size);
        }

        private void NewsBulletinsEvent()
        {
            var msgVersion = ReadInt();
            var newsMsgId = ReadInt();
            var newsMsgType = ReadInt();
            var newsMessage = ReadString();
            var originatingExch = ReadString();
            eWrapper.UpdateNewsBulletin(newsMsgId, newsMsgType, newsMessage, originatingExch);
        }

        private void PositionEvent()
        {
            var msgVersion = ReadInt();
            var account = ReadString();
            var contract = new Contract();
            contract.ConId = ReadInt();
            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            contract.Multiplier = ReadString();
            contract.Exchange = ReadString();
            contract.Currency = ReadString();
            contract.LocalSymbol = ReadString();
            if (msgVersion >= 2)
            {
                contract.TradingClass = ReadString();
            }

            var pos = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            double avgCost = 0;
            if (msgVersion >= 3)
            {
                avgCost = ReadDouble();
            }

            eWrapper.Position(account, contract, pos, avgCost);
        }

        private void PositionEndEvent()
        {
            var msgVersion = ReadInt();
            eWrapper.PositionEnd();
        }

        private void RealTimeBarsEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var time = ReadLong();
            var open = ReadDouble();
            var high = ReadDouble();
            var low = ReadDouble();
            var close = ReadDouble();
            var volume = ReadLong();
            var wap = ReadDouble();
            var count = ReadInt();
            eWrapper.RealtimeBar(requestId, time, open, high, low, close, volume, wap, count);
        }

        private void ScannerParametersEvent()
        {
            var msgVersion = ReadInt();
            var xml = ReadString();
            eWrapper.ScannerParameters(xml);
        }

        private void ScannerDataEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var numberOfElements = ReadInt();
            for (var i = 0; i < numberOfElements; i++)
            {
                var rank = ReadInt();
                var conDet = new ContractDetails();
                if (msgVersion >= 3)
                {
                    conDet.Contract.ConId = ReadInt();
                }

                conDet.Contract.Symbol = ReadString();
                conDet.Contract.SecType = ReadString();
                conDet.Contract.LastTradeDateOrContractMonth = ReadString();
                conDet.Contract.Strike = ReadDouble();
                conDet.Contract.Right = ReadString();
                conDet.Contract.Exchange = ReadString();
                conDet.Contract.Currency = ReadString();
                conDet.Contract.LocalSymbol = ReadString();
                conDet.MarketName = ReadString();
                conDet.Contract.TradingClass = ReadString();
                var distance = ReadString();
                var benchmark = ReadString();
                var projection = ReadString();
                string legsStr = null;
                if (msgVersion >= 2)
                {
                    legsStr = ReadString();
                }

                eWrapper.ScannerData(requestId,
                                     rank,
                                     conDet,
                                     distance,
                                     benchmark,
                                     projection,
                                     legsStr);
            }

            eWrapper.ScannerDataEnd(requestId);
        }

        private void ReceiveFaEvent()
        {
            var msgVersion = ReadInt();
            var faDataType = ReadInt();
            var faData = ReadString();
            eWrapper.ReceiveFa(faDataType, faData);
        }

        private void PositionMultiEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var account = ReadString();
            var contract = new Contract();
            contract.ConId = ReadInt();
            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            contract.Multiplier = ReadString();
            contract.Exchange = ReadString();
            contract.Currency = ReadString();
            contract.LocalSymbol = ReadString();
            contract.TradingClass = ReadString();
            var pos = ReadDouble();
            var avgCost = ReadDouble();
            var modelCode = ReadString();
            eWrapper.PositionMulti(requestId, account, modelCode, contract, pos, avgCost);
        }

        private void PositionMultiEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.PositionMultiEnd(requestId);
        }

        private void AccountUpdateMultiEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var account = ReadString();
            var modelCode = ReadString();
            var key = ReadString();
            var value = ReadString();
            var currency = ReadString();
            eWrapper.AccountUpdateMulti(requestId, account, modelCode, key, value, currency);
        }

        private void AccountUpdateMultiEndEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            eWrapper.AccountUpdateMultiEnd(requestId);
        }

        public char ReadChar()
        {
            var str = ReadString();
            return str == null ? '\0' : str[0];
        }

        private void ReadLastTradeDate(ContractDetails contract, bool isBond)
        {
            var lastTradeDateOrContractMonth = ReadString();
            if (lastTradeDateOrContractMonth != null)
            {
                var splitted = Regex.Split(lastTradeDateOrContractMonth, "\\s+");
                if (splitted.Length > 0)
                {
                    if (isBond)
                    {
                        contract.Maturity = splitted[0];
                    }
                    else
                    {
                        contract.Contract.LastTradeDateOrContractMonth = splitted[0];
                    }
                }

                if (splitted.Length > 1)
                {
                    contract.LastTradeTime = splitted[1];
                }

                if (isBond && splitted.Length > 2)
                {
                    contract.TimeZoneId = splitted[2];
                }
            }
        }
    }
}