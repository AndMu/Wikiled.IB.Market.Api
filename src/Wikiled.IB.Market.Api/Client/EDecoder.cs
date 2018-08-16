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
            string doubleAsstring = ReadString();
            if (string.IsNullOrEmpty(doubleAsstring) ||
                doubleAsstring == "0")
            {
                return 0;
            }

            return double.Parse(doubleAsstring, NumberFormatInfo.InvariantInfo);
        }

        public double ReadDoubleMax()
        {
            string str = ReadString();
            return str == null || str.Length == 0 ? double.MaxValue : double.Parse(str, NumberFormatInfo.InvariantInfo);
        }

        public long ReadLong()
        {
            string longAsstring = ReadString();
            if (string.IsNullOrEmpty(longAsstring) ||
                longAsstring == "0")
            {
                return 0;
            }

            return long.Parse(longAsstring);
        }

        public int ReadInt()
        {
            string intAsstring = ReadString();
            if (string.IsNullOrEmpty(intAsstring) ||
                intAsstring == "0")
            {
                return 0;
            }

            return int.Parse(intAsstring);
        }

        public int ReadIntMax()
        {
            string str = ReadString();
            return str == null || str.Length == 0 ? int.MaxValue : int.Parse(str);
        }

        public bool ReadBoolFromInt()
        {
            string str = ReadString();
            return str != null && int.Parse(str) != 0;
        }

        public T ReadEnum<T>()
            where T : struct
        {
            string text = ReadString();
            if (!Enum.TryParse(text, true, out T value))
            {
                throw new ArgumentOutOfRangeException($"Can't parse {text} to {typeof(T)}");
            }

            return value;
        }

        public string ReadString()
        {
            byte b = dataReader.ReadByte();

            nDecodedLen++;

            if (b == 0)
            {
                return null;
            }

            StringBuilder strBuilder = new StringBuilder();
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
            dataReader?.Dispose();
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
                string srv = ReadString();

                serverVersion = 0;

                if (eClientMsgSink != null)
                {
                    eClientMsgSink.Redirect(srv);
                }

                return;
            }

            string serverTime = "";

            if (serverVersion >= 20)
            {
                serverTime = ReadString();
            }

            eClientMsgSink?.ServerVersion(serverVersion, serverTime);

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
            int reqId = ReadInt();
            int tickType = ReadInt();
            long time = ReadLong();
            BitMask mask;
            TickAttrib attribs;

            switch (tickType)
            {
                case 0: // None
                    break;
                case 1: // Last
                case 2: // AllLast
                    double price = ReadDouble();
                    int size = ReadInt();
                    mask = new BitMask(ReadInt());
                    attribs = new TickAttrib
                    {
                        PastLimit = mask[0],
                        Unreported = mask[1]
                    };
                    string exchange = ReadString();
                    string specialConditions = ReadString();
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
                    double bidPrice = ReadDouble();
                    double askPrice = ReadDouble();
                    int bidSize = ReadInt();
                    int askSize = ReadInt();
                    mask = new BitMask(ReadInt());
                    attribs = new TickAttrib
                    {
                        BidPastLow = mask[0],
                        AskPastHigh = mask[1]
                    };
                    eWrapper.TickByTickBidAsk(reqId, time, bidPrice, askPrice, bidSize, askSize, attribs);
                    break;
                case 4: // MidPoint
                    double midPoint = ReadDouble();
                    eWrapper.TickByTickMidPoint(reqId, time, midPoint);
                    break;
            }
        }

        private void HistoricalTickLastEvent()
        {
            int reqId = ReadInt();
            int nTicks = ReadInt();
            HistoricalTickLast[] ticks = new HistoricalTickLast[nTicks];

            for (int i = 0; i < nTicks; i++)
            {
                long time = ReadLong();
                int mask = ReadInt();
                double price = ReadDouble();
                long size = ReadLong();
                string exchange = ReadString();
                string specialConditions = ReadString();

                ticks[i] = new HistoricalTickLast(time, mask, price, size, exchange, specialConditions);
            }

            bool done = ReadBoolFromInt();

            eWrapper.HistoricalTicksLast(reqId, ticks, done);
        }

        private void HistoricalTickBidAskEvent()
        {
            int reqId = ReadInt();
            int nTicks = ReadInt();
            HistoricalTickBidAsk[] ticks = new HistoricalTickBidAsk[nTicks];

            for (int i = 0; i < nTicks; i++)
            {
                long time = ReadLong();
                int mask = ReadInt();
                double priceBid = ReadDouble();
                double priceAsk = ReadDouble();
                long sizeBid = ReadLong();
                long sizeAsk = ReadLong();

                ticks[i] = new HistoricalTickBidAsk(time, mask, priceBid, priceAsk, sizeBid, sizeAsk);
            }

            bool done = ReadBoolFromInt();

            eWrapper.HistoricalTicksBidAsk(reqId, ticks, done);
        }

        private void HistoricalTickEvent()
        {
            int reqId = ReadInt();
            int nTicks = ReadInt();
            HistoricalTick[] ticks = new HistoricalTick[nTicks];

            for (int i = 0; i < nTicks; i++)
            {
                long time = ReadLong();
                ReadInt(); // for consistency
                double price = ReadDouble();
                long size = ReadLong();

                ticks[i] = new HistoricalTick(time, price, size);
            }

            bool done = ReadBoolFromInt();

            eWrapper.HistoricalTicks(reqId, ticks, done);
        }

        private void MarketRuleEvent()
        {
            int marketRuleId = ReadInt();
            PriceIncrement[] priceIncrements = new PriceIncrement[0];
            int nPriceIncrements = ReadInt();

            if (nPriceIncrements > 0)
            {
                Array.Resize(ref priceIncrements, nPriceIncrements);

                for (int i = 0; i < nPriceIncrements; ++i)
                {
                    priceIncrements[i] = new PriceIncrement(ReadDouble(), ReadDouble());
                }
            }

            eWrapper.MarketRule(marketRuleId, priceIncrements);
        }

        private void RerouteMktDepthReqEvent()
        {
            int reqId = ReadInt();
            int conId = ReadInt();
            string exchange = ReadString();

            eWrapper.RerouteMktDepthReq(reqId, conId, exchange);
        }

        private void RerouteMktDataReqEvent()
        {
            int reqId = ReadInt();
            int conId = ReadInt();
            string exchange = ReadString();

            eWrapper.RerouteMktDataReq(reqId, conId, exchange);
        }

        private void HistoricalDataUpdateEvent()
        {
            int requestId = ReadInt();
            int barCount = ReadInt();
            string date = ReadString();
            double open = ReadDouble();
            double close = ReadDouble();
            double high = ReadDouble();
            double low = ReadDouble();
            double wap = ReadDouble();
            long volume = ReadLong();

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
            int reqId = ReadInt();
            int pos = ReadInt();
            double dailyPnL = ReadDouble();
            double unrealizedPnL = double.MaxValue;
            double realizedPnL = double.MaxValue;

            if (serverVersion >= MinServerVer.UnrealizedPnl)
            {
                unrealizedPnL = ReadDouble();
            }

            if (serverVersion >= MinServerVer.RealizedPnl)
            {
                realizedPnL = ReadDouble();
            }

            double value = ReadDouble();

            eWrapper.PnlSingle(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value);
        }

        private void PnLEvent()
        {
            int reqId = ReadInt();
            double dailyPnL = ReadDouble();
            double unrealizedPnL = double.MaxValue;
            double realizedPnL = double.MaxValue;

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
            int reqId = ReadInt();
            int n = ReadInt();
            HistogramEntry[] data = new HistogramEntry[n];

            for (int i = 0; i < n; i++)
            {
                data[i].Price = ReadDouble();
                data[i].Size = ReadLong();
            }

            eWrapper.HistogramData(reqId, data);
        }

        private void HeadTimestampEvent()
        {
            int reqId = ReadInt();
            string headTimestamp = ReadString();

            eWrapper.HeadTimestamp(reqId, headTimestamp);
        }

        private void HistoricalNewsEvent()
        {
            int requestId = ReadInt();
            string time = ReadString();
            string providerCode = ReadString();
            string articleId = ReadString();
            string headline = ReadString();

            eWrapper.HistoricalNews(requestId, time, providerCode, articleId, headline);
        }

        private void HistoricalNewsEndEvent()
        {
            int requestId = ReadInt();
            bool hasMore = ReadBoolFromInt();

            eWrapper.HistoricalNewsEnd(requestId, hasMore);
        }

        private void NewsArticleEvent()
        {
            int requestId = ReadInt();
            int articleType = ReadInt();
            string articleText = ReadString();

            eWrapper.NewsArticle(requestId, articleType, articleText);
        }

        private void NewsProvidersEvent()
        {
            NewsProvider[] newsProviders = new NewsProvider[0];
            int nNewsProviders = ReadInt();

            if (nNewsProviders > 0)
            {
                Array.Resize(ref newsProviders, nNewsProviders);

                for (int i = 0; i < nNewsProviders; ++i)
                {
                    newsProviders[i] = new NewsProvider(ReadString(), ReadString());
                }
            }

            eWrapper.NewsProviders(newsProviders);
        }

        private void SmartComponentsEvent()
        {
            int reqId = ReadInt();
            int n = ReadInt();
            Dictionary<int, KeyValuePair<string, char>> theMap = new Dictionary<int, KeyValuePair<string, char>>();

            for (int i = 0; i < n; i++)
            {
                int bitNumber = ReadInt();
                string exchange = ReadString();
                char exchangeLetter = ReadChar();

                theMap.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }

            eWrapper.SmartComponents(reqId, theMap);
        }

        private void TickReqParamsEvent()
        {
            int tickerId = ReadInt();
            double minTick = ReadDouble();
            string bboExchange = ReadString();
            int snapshotPermissions = ReadInt();

            eWrapper.TickReqParams(tickerId, minTick, bboExchange, snapshotPermissions);
        }

        private void TickNewsEvent()
        {
            int tickerId = ReadInt();
            long timeStamp = ReadLong();
            string providerCode = ReadString();
            string articleId = ReadString();
            string headline = ReadString();
            string extraData = ReadString();

            eWrapper.TickNews(tickerId, timeStamp, providerCode, articleId, headline, extraData);
        }

        private void SymbolSamplesEvent()
        {
            int reqId = ReadInt();
            ContractDescription[] contractDescriptions = new ContractDescription[0];
            int nContractDescriptions = ReadInt();

            if (nContractDescriptions > 0)
            {
                Array.Resize(ref contractDescriptions, nContractDescriptions);

                for (int i = 0; i < nContractDescriptions; ++i)
                {
                    // read contract fields
                    Contract contract = new Contract
                    {
                        ConId = ReadInt(),
                        Symbol = ReadString(),
                        SecType = ReadEnum<SecType>(),
                        PrimaryExch = ReadString(),
                        Currency = ReadString()
                    };

                    // read derivative sec types list
                    string[] derivativeSecTypes = new string[0];
                    int nDerivativeSecTypes = ReadInt();
                    if (nDerivativeSecTypes > 0)
                    {
                        Array.Resize(ref derivativeSecTypes, nDerivativeSecTypes);
                        for (int j = 0; j < nDerivativeSecTypes; ++j)
                        {
                            derivativeSecTypes[j] = ReadString();
                        }
                    }

                    ContractDescription contractDescription = new ContractDescription(contract, derivativeSecTypes);
                    contractDescriptions[i] = contractDescription;
                }
            }

            eWrapper.SymbolSamples(reqId, contractDescriptions);
        }

        private void FamilyCodesEvent()
        {
            FamilyCode[] familyCodes = new FamilyCode[0];
            int nFamilyCodes = ReadInt();

            if (nFamilyCodes > 0)
            {
                Array.Resize(ref familyCodes, nFamilyCodes);

                for (int i = 0; i < nFamilyCodes; ++i)
                {
                    familyCodes[i] = new FamilyCode(ReadString(), ReadString());
                }
            }

            eWrapper.FamilyCodes(familyCodes);
        }

        private void MktDepthExchangesEvent()
        {
            DepthMktDataDescription[] depthMktDataDescriptions = new DepthMktDataDescription[0];
            int nDescriptions = ReadInt();

            if (nDescriptions > 0)
            {
                Array.Resize(ref depthMktDataDescriptions, nDescriptions);

                for (int i = 0; i < nDescriptions; i++)
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
            int reqId = ReadInt();
            int nTiers = ReadInt();
            SoftDollarTier[] tiers = new SoftDollarTier[nTiers];

            for (int i = 0; i < nTiers; i++)
            {
                tiers[i] = new SoftDollarTier(ReadString(), ReadString(), ReadString());
            }

            eWrapper.SoftDollarTiers(reqId, tiers);
        }

        private void SecurityDefinitionOptionParameterEndEvent()
        {
            int reqId = ReadInt();

            eWrapper.SecurityDefinitionOptionParameterEnd(reqId);
        }

        private void SecurityDefinitionOptionParameterEvent()
        {
            int reqId = ReadInt();
            string exchange = ReadString();
            int underlyingConId = ReadInt();
            string tradingClass = ReadString();
            string multiplier = ReadString();
            int expirationsSize = ReadInt();
            HashSet<string> expirations = new HashSet<string>();
            HashSet<double> strikes = new HashSet<double>();

            for (int i = 0; i < expirationsSize; i++)
            {
                expirations.Add(ReadString());
            }

            int strikesSize = ReadInt();

            for (int i = 0; i < strikesSize; i++)
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
            int msgVersion = ReadInt();
            int reqId = ReadInt();
            string contractInfo = ReadString();

            eWrapper.DisplayGroupUpdated(reqId, contractInfo);
        }

        private void DisplayGroupListEvent()
        {
            int msgVersion = ReadInt();
            int reqId = ReadInt();
            string groups = ReadString();

            eWrapper.DisplayGroupList(reqId, groups);
        }

        private void VerifyCompletedEvent()
        {
            int msgVersion = ReadInt();
            bool isSuccessful = string.Compare(ReadString(), "true", true) == 0;
            string errorText = ReadString();

            eWrapper.VerifyCompleted(isSuccessful, errorText);
        }

        private void VerifyMessageApiEvent()
        {
            int msgVersion = ReadInt();
            string apiData = ReadString();

            eWrapper.VerifyMessageApi(apiData);
        }

        private void VerifyAndAuthCompletedEvent()
        {
            int msgVersion = ReadInt();
            bool isSuccessful = string.Compare(ReadString(), "true", true) == 0;
            string errorText = ReadString();

            eWrapper.VerifyAndAuthCompleted(isSuccessful, errorText);
        }

        private void VerifyAndAuthMessageApiEvent()
        {
            int msgVersion = ReadInt();
            string apiData = ReadString();
            string xyzChallenge = ReadString();

            eWrapper.VerifyAndAuthMessageApi(apiData, xyzChallenge);
        }

        private void TickPriceEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            double price = ReadDouble();
            int size = 0;

            if (msgVersion >= 2)
            {
                size = ReadInt();
            }

            TickAttrib attr = new TickAttrib();

            if (msgVersion >= 3)
            {
                int attrMask = ReadInt();

                attr.CanAutoExecute = attrMask == 1;

                if (serverVersion >= MinServerVer.PastLimit)
                {
                    BitMask mask = new BitMask(attrMask);

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
                int sizeTickType = -1; //not a tick
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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            int size = ReadInt();
            eWrapper.TickSize(requestId, tickType, size);
        }

        private void TickStringEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            string value = ReadString();
            eWrapper.TickString(requestId, tickType, value);
        }

        private void TickGenericEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            double value = ReadDouble();
            eWrapper.TickGeneric(requestId, tickType, value);
        }

        private void TickEfpEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            double basisPoints = ReadDouble();
            string formattedBasisPoints = ReadString();
            double impliedFuturesPrice = ReadDouble();
            int holdDays = ReadInt();
            string futureLastTradeDate = ReadString();
            double dividendImpact = ReadDouble();
            double dividendsToLastTradeDate = ReadDouble();
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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.TickSnapshotEnd(requestId);
        }

        private void ErrorEvent()
        {
            int msgVersion = ReadInt();
            if (msgVersion < 2)
            {
                string msg = ReadString();
                eWrapper.Error(msg);
            }
            else
            {
                int id = ReadInt();
                int errorCode = ReadInt();
                string errorMsg = ReadString();
                eWrapper.Error(id, errorCode, errorMsg);
            }
        }

        private void CurrentTimeEvent()
        {
            int msgVersion = ReadInt(); //version
            long time = ReadLong();
            eWrapper.CurrentTime(time);
        }

        private void ManagedAccountsEvent()
        {
            int msgVersion = ReadInt();
            string accountsList = ReadString();
            eWrapper.ManagedAccounts(accountsList);
        }

        private void NextValidIdEvent()
        {
            int msgVersion = ReadInt();
            int orderId = ReadInt();
            eWrapper.NextValidId(orderId);
        }

        private void DeltaNeutralValidationEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            DeltaNeutralContract deltaNeutralContract = new DeltaNeutralContract
            {
                ConId = ReadInt(),
                Delta = ReadDouble(),
                Price = ReadDouble()
            };
            eWrapper.DeltaNeutralValidation(requestId, deltaNeutralContract);
        }

        private void TickOptionComputationEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int tickType = ReadInt();
            double impliedVolatility = ReadDouble();
            if (impliedVolatility.Equals(-1))
            {
                // -1 is the "not yet computed" indicator
                impliedVolatility = double.MaxValue;
            }

            double delta = ReadDouble();
            if (delta.Equals(-2))
            {
                // -2 is the "not yet computed" indicator
                delta = double.MaxValue;
            }

            double optPrice = double.MaxValue;
            double pvDividend = double.MaxValue;
            double gamma = double.MaxValue;
            double vega = double.MaxValue;
            double theta = double.MaxValue;
            double undPrice = double.MaxValue;
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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            string account = ReadString();
            string tag = ReadString();
            string value = ReadString();
            string currency = ReadString();
            eWrapper.AccountSummary(requestId, account, tag, value, currency);
        }

        private void AccountSummaryEndEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.AccountSummaryEnd(requestId);
        }

        private void AccountValueEvent()
        {
            int msgVersion = ReadInt();
            string key = ReadString();
            string value = ReadString();
            string currency = ReadString();
            string accountName = null;
            if (msgVersion >= 2)
            {
                accountName = ReadString();
            }

            eWrapper.UpdateAccountValue(key, value, currency, accountName);
        }

        private void BondContractDetailsEvent()
        {
            int msgVersion = ReadInt();
            int requestId = -1;
            if (msgVersion >= 3)
            {
                requestId = ReadInt();
            }

            ContractDetails contract = new ContractDetails();

            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadEnum<SecType>();
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
            contract.Contract.Exchange = (ExchangeType)Enum.Parse(typeof(ExchangeType), ReadString());
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
                int secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>();
                    for (int i = 0; i < secIdListCount; ++i)
                    {
                        TagValue tagValue = new TagValue
                        {
                            Tag = ReadString(),
                            Value = ReadString()
                        };
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
            int msgVersion = ReadInt();
            Contract contract = new Contract();
            if (msgVersion >= 6)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadEnum<SecType>();
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

            double position = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            double marketPrice = ReadDouble();
            double marketValue = ReadDouble();
            double averageCost = 0.0;
            double unrealizedPnl = 0.0;
            double realizedPnl = 0.0;
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
            int msgVersion = ReadInt();
            string timestamp = ReadString();
            eWrapper.UpdateAccountTime(timestamp);
        }

        private void AccountDownloadEndEvent()
        {
            int msgVersion = ReadInt();
            string account = ReadString();
            eWrapper.AccountDownloadEnd(account);
        }

        private void OrderStatusEvent()
        {
            int msgVersion = serverVersion >= MinServerVer.MarketCapPrice ? int.MaxValue : ReadInt();
            int id = ReadInt();
            string status = ReadString();
            double filled = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            double remaining = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            double avgFillPrice = ReadDouble();

            int permId = 0;
            if (msgVersion >= 2)
            {
                permId = ReadInt();
            }

            int parentId = 0;
            if (msgVersion >= 3)
            {
                parentId = ReadInt();
            }

            double lastFillPrice = 0;
            if (msgVersion >= 4)
            {
                lastFillPrice = ReadDouble();
            }

            int clientId = 0;
            if (msgVersion >= 5)
            {
                clientId = ReadInt();
            }

            string whyHeld = null;
            if (msgVersion >= 6)
            {
                whyHeld = ReadString();
            }

            double mktCapPrice = double.MaxValue;

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
            int msgVersion = ReadInt();
            // read order id
            Order order = new Order
            {
                OrderId = ReadInt()
            };

            // read contract fields
            Contract contract = new Contract();
            if (msgVersion >= 17)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadEnum<SecType>();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 32)
            {
                contract.Multiplier = ReadString();
            }

            contract.Exchange = ReadEnum<ExchangeType>();
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
                    int receivedInt = ReadInt();
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
                int comboLegsCount = ReadInt();
                if (comboLegsCount > 0)
                {
                    contract.ComboLegs = new List<ComboLeg>(comboLegsCount);
                    for (int i = 0; i < comboLegsCount; ++i)
                    {
                        int conId = ReadInt();
                        int ratio = ReadInt();
                        string action = ReadString();
                        string exchange = ReadString();
                        int openClose = ReadInt();
                        int shortSaleSlot = ReadInt();
                        string designatedLocation = ReadString();
                        int exemptCode = ReadInt();

                        ComboLeg comboLeg = new ComboLeg(conId,
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

                int orderComboLegsCount = ReadInt();
                if (orderComboLegsCount > 0)
                {
                    order.OrderComboLegs = new List<OrderComboLeg>(orderComboLegsCount);
                    for (int i = 0; i < orderComboLegsCount; ++i)
                    {
                        double price = ReadDoubleMax();

                        OrderComboLeg orderComboLeg = new OrderComboLeg(price);
                        order.OrderComboLegs.Add(orderComboLeg);
                    }
                }
            }

            if (msgVersion >= 26)
            {
                int smartComboRoutingParamsCount = ReadInt();
                if (smartComboRoutingParamsCount > 0)
                {
                    order.SmartComboRoutingParams = new List<TagValue>(smartComboRoutingParamsCount);
                    for (int i = 0; i < smartComboRoutingParamsCount; ++i)
                    {
                        TagValue tagValue = new TagValue
                        {
                            Tag = ReadString(),
                            Value = ReadString()
                        };
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
                    DeltaNeutralContract deltaNeutralContract = new DeltaNeutralContract
                    {
                        ConId = ReadInt(),
                        Delta = ReadDouble(),
                        Price = ReadDouble()
                    };
                    contract.DeltaNeutralContract = deltaNeutralContract;
                }
            }

            if (msgVersion >= 21)
            {
                order.AlgoStrategy = ReadString();
                if (!Util.StringIsEmpty(order.AlgoStrategy))
                {
                    int algoParamsCount = ReadInt();
                    if (algoParamsCount > 0)
                    {
                        order.AlgoParams = new List<TagValue>(algoParamsCount);
                        for (int i = 0; i < algoParamsCount; ++i)
                        {
                            TagValue tagValue = new TagValue
                            {
                                Tag = ReadString(),
                                Value = ReadString()
                            };
                            order.AlgoParams.Add(tagValue);
                        }
                    }
                }
            }

            if (msgVersion >= 33)
            {
                order.Solicited = ReadBoolFromInt();
            }

            OrderState orderState = new OrderState();
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

                int nConditions = ReadInt();

                if (nConditions > 0)
                {
                    for (int i = 0; i < nConditions; i++)
                    {
                        OrderConditionType orderConditionType = (OrderConditionType)ReadInt();
                        OrderCondition condition = OrderCondition.Create(orderConditionType);

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
            int msgVersion = ReadInt();
            eWrapper.OpenOrderEnd();
        }

        private void ContractDataEvent()
        {
            int msgVersion = ReadInt();
            int requestId = -1;
            if (msgVersion >= 3)
            {
                requestId = ReadInt();
            }

            ContractDetails contract = new ContractDetails();
            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadEnum<SecType>();
            ReadLastTradeDate(contract, false);
            contract.Contract.Strike = ReadDouble();
            contract.Contract.Right = ReadString();
            contract.Contract.Exchange = ReadEnum<ExchangeType>();
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
                int secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>(secIdListCount);
                    for (int i = 0; i < secIdListCount; ++i)
                    {
                        TagValue tagValue = new TagValue
                        {
                            Tag = ReadString(),
                            Value = ReadString()
                        };
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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.ContractDetailsEnd(requestId);
        }

        private void ExecutionDataEvent()
        {
            int msgVersion = serverVersion;

            if (serverVersion < MinServerVer.LastLiquidity)
            {
                msgVersion = ReadInt();
            }

            int requestId = -1;
            if (msgVersion >= 7)
            {
                requestId = ReadInt();
            }

            int orderId = ReadInt();
            Contract contract = new Contract();
            if (msgVersion >= 5)
            {
                contract.ConId = ReadInt();
            }

            contract.Symbol = ReadString();
            contract.SecType = ReadEnum<SecType>();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 9)
            {
                contract.Multiplier = ReadString();
            }

            contract.Exchange = ReadEnum<ExchangeType>();
            contract.Currency = ReadString();
            contract.LocalSymbol = ReadString();
            if (msgVersion >= 10)
            {
                contract.TradingClass = ReadString();
            }

            Execution exec = new Execution
            {
                OrderId = orderId,
                ExecId = ReadString(),
                Time = ReadString(),
                AcctNumber = ReadString(),
                Exchange = ReadString(),
                Side = ReadString(),
                Shares = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt(),
                Price = ReadDouble()
            };
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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.ExecDetailsEnd(requestId);
        }

        private void CommissionReportEvent()
        {
            int msgVersion = ReadInt();
            CommissionReport commissionReport = new CommissionReport
            {
                ExecId = ReadString(),
                Commission = ReadDouble(),
                Currency = ReadString(),
                RealizedPnl = ReadDouble(),
                Yield = ReadDouble(),
                YieldRedemptionDate = ReadInt()
            };
            eWrapper.CommissionReport(commissionReport);
        }

        private void FundamentalDataEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            string fundamentalData = ReadString();
            eWrapper.FundamentalData(requestId, fundamentalData);
        }

        private void HistoricalDataEvent()
        {
            int msgVersion = int.MaxValue;

            if (serverVersion < MinServerVer.SyntRealtimeBars)
            {
                msgVersion = ReadInt();
            }

            int requestId = ReadInt();
            string startDateStr = "";
            string endDateStr = "";

            if (msgVersion >= 2)
            {
                startDateStr = ReadString();
                endDateStr = ReadString();
            }

            int itemCount = ReadInt();

            for (int ctr = 0; ctr < itemCount; ctr++)
            {
                string date = ReadString();
                double open = ReadDouble();
                double high = ReadDouble();
                double low = ReadDouble();
                double close = ReadDouble();
                long volume = serverVersion < MinServerVer.SyntRealtimeBars ? ReadInt() : ReadLong();
                double wap = ReadDouble();

                if (serverVersion < MinServerVer.SyntRealtimeBars)
                {
                    /*string hasGaps = */
                    ReadString();
                }

                int barCount = -1;

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
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int marketDataType = ReadInt();
            eWrapper.MarketDataType(requestId, marketDataType);
        }

        private void MarketDepthEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int position = ReadInt();
            int operation = ReadInt();
            int side = ReadInt();
            double price = ReadDouble();
            int size = ReadInt();
            eWrapper.UpdateMktDepth(requestId, position, operation, side, price, size);
        }

        private void MarketDepthL2Event()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int position = ReadInt();
            string marketMaker = ReadString();
            int operation = ReadInt();
            int side = ReadInt();
            double price = ReadDouble();
            int size = ReadInt();
            eWrapper.UpdateMktDepthL2(requestId, position, marketMaker, operation, side, price, size);
        }

        private void NewsBulletinsEvent()
        {
            int msgVersion = ReadInt();
            int newsMsgId = ReadInt();
            int newsMsgType = ReadInt();
            string newsMessage = ReadString();
            string originatingExch = ReadString();
            eWrapper.UpdateNewsBulletin(newsMsgId, newsMsgType, newsMessage, originatingExch);
        }

        private void PositionEvent()
        {
            int msgVersion = ReadInt();
            string account = ReadString();
            Contract contract = new Contract
            {
                ConId = ReadInt(),
                Symbol = ReadString(),
                SecType = ReadEnum<SecType>(),
                LastTradeDateOrContractMonth = ReadString(),
                Strike = ReadDouble(),
                Right = ReadString(),
                Multiplier = ReadString(),
                Exchange = ReadEnum<ExchangeType>(),
                Currency = ReadString(),
                LocalSymbol = ReadString()
            };
            if (msgVersion >= 2)
            {
                contract.TradingClass = ReadString();
            }

            double pos = serverVersion >= MinServerVer.FractionalPositions ? ReadDouble() : ReadInt();
            double avgCost = 0;
            if (msgVersion >= 3)
            {
                avgCost = ReadDouble();
            }

            eWrapper.Position(account, contract, pos, avgCost);
        }

        private void PositionEndEvent()
        {
            int msgVersion = ReadInt();
            eWrapper.PositionEnd();
        }

        private void RealTimeBarsEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            long time = ReadLong();
            double open = ReadDouble();
            double high = ReadDouble();
            double low = ReadDouble();
            double close = ReadDouble();
            long volume = ReadLong();
            double wap = ReadDouble();
            int count = ReadInt();
            eWrapper.RealtimeBar(requestId, time, open, high, low, close, volume, wap, count);
        }

        private void ScannerParametersEvent()
        {
            int msgVersion = ReadInt();
            string xml = ReadString();
            eWrapper.ScannerParameters(xml);
        }

        private void ScannerDataEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            int numberOfElements = ReadInt();
            for (int i = 0; i < numberOfElements; i++)
            {
                int rank = ReadInt();
                ContractDetails conDet = new ContractDetails();
                if (msgVersion >= 3)
                {
                    conDet.Contract.ConId = ReadInt();
                }

                conDet.Contract.Symbol = ReadString();
                conDet.Contract.SecType = ReadEnum<SecType>();
                conDet.Contract.LastTradeDateOrContractMonth = ReadString();
                conDet.Contract.Strike = ReadDouble();
                conDet.Contract.Right = ReadString();
                conDet.Contract.Exchange = ReadEnum<ExchangeType>();
                conDet.Contract.Currency = ReadString();
                conDet.Contract.LocalSymbol = ReadString();
                conDet.MarketName = ReadString();
                conDet.Contract.TradingClass = ReadString();
                string distance = ReadString();
                string benchmark = ReadString();
                string projection = ReadString();
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
            int msgVersion = ReadInt();
            int faDataType = ReadInt();
            string faData = ReadString();
            eWrapper.ReceiveFa(faDataType, faData);
        }

        private void PositionMultiEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            string account = ReadString();
            Contract contract = new Contract
            {
                ConId = ReadInt(),
                Symbol = ReadString(),
                SecType = ReadEnum<SecType>(),
                LastTradeDateOrContractMonth = ReadString(),
                Strike = ReadDouble(),
                Right = ReadString(),
                Multiplier = ReadString(),
                Exchange = ReadEnum<ExchangeType>(),
                Currency = ReadString(),
                LocalSymbol = ReadString(),
                TradingClass = ReadString()
            };
            double pos = ReadDouble();
            double avgCost = ReadDouble();
            string modelCode = ReadString();
            eWrapper.PositionMulti(requestId, account, modelCode, contract, pos, avgCost);
        }

        private void PositionMultiEndEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.PositionMultiEnd(requestId);
        }

        private void AccountUpdateMultiEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            string account = ReadString();
            string modelCode = ReadString();
            string key = ReadString();
            string value = ReadString();
            string currency = ReadString();
            eWrapper.AccountUpdateMulti(requestId, account, modelCode, key, value, currency);
        }

        private void AccountUpdateMultiEndEvent()
        {
            int msgVersion = ReadInt();
            int requestId = ReadInt();
            eWrapper.AccountUpdateMultiEnd(requestId);
        }

        public char ReadChar()
        {
            string str = ReadString();
            return str == null ? '\0' : str[0];
        }

        private void ReadLastTradeDate(ContractDetails contract, bool isBond)
        {
            string lastTradeDateOrContractMonth = ReadString();
            if (lastTradeDateOrContractMonth != null)
            {
                string[] splitted = Regex.Split(lastTradeDateOrContractMonth, "\\s+");
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