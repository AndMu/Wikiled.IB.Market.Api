using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class EClient
     * @brief TWS/Gateway client class
     * This client class contains all the available methods to communicate with IB. Up to thirty-two clients can be connected to a single instance of the TWS/Gateway simultaneously. From herein, the TWS/Gateway will be referred to as the Host.
     */
    public abstract class EClient
    {
        /**
         * @brief Establishes a connection to the designated Host.
         * After establishing a connection successfully, the Host will provide the next valid order id, server's current time, managed accounts and open orders among others depending on the Host version.
         * @param host the Host's IP address. Leave blank for localhost.
         * @param port the Host's port. 7496 by default for the TWS, 4001 by default on the Gateway.
         * @param clientId Every API client program requires a unique id which can be any integer. Note that up to 32 clients can be connected simultaneously to a single Host.
         * @sa EWrapper, EWrapper::nextValidId, EWrapper::currentTime
         */

        private static readonly string EncodedVersion = Constants.MinVersion + (Constants.MaxVersion != Constants.MinVersion ? ".." + Constants.MaxVersion : string.Empty);

        protected int ClientId;

        private string connectOptions = "";

        protected bool ExtraAuth;

        protected IETransport SocketTransport;

        protected Stream TcpStream;

        /**
         * @brief Constructor
         * @param Wrapper EWrapper's implementing class instance. Every message being delivered by IB to the API client will be forwarded to the EWrapper's implementing class.
         * @sa EWrapper
         */
        protected EClient(IEWrapper Wrapper)
        {
            Wrapper = Wrapper;
            ClientId = -1;
            ExtraAuth = false;
            IsConnected = false;
            OptionalCapabilities = "";
            AsyncEConnect = false;
            UseV100Plus = true;
        }

        internal bool UseV100Plus { get; private set; }

        /**
         * @brief Reference to the EWrapper implementing object.
         */
        public IEWrapper Wrapper {get; }

        public bool AllowRedirect { get; set; }

        /**
         * @brief returns the Host's version. Some of the API functionality might not be available in older Hosts and therefore it is essential to keep the TWS/Gateway as up to date as possible.
         */
        public int ServerVersion { get; protected set; }

        public string ServerTime { get; protected set; }


        public string OptionalCapabilities { get; set; }

        public bool AsyncEConnect { get; set; }

        /**
         * @brief Ignore. Used for IB's internal purposes.
         */
        public void SetConnectOptions(string connectOptions)
        {
            if (IsConnected)
            {
                Wrapper.Error(ClientId, EClientErrors.AlreadyConnected.Code, EClientErrors.AlreadyConnected.Message);
                return;
            }

            this.connectOptions = connectOptions;
        }

        /**
         * @brief Allows to switch between different current (V100+) and previous connection mechanisms.
         */
        public void DisableUseV100Plus()
        {
            UseV100Plus = false;
            connectOptions = "";
        }

        /**
         * @brief Indicates whether the API-TWS connection has been closed. 
         * Note: This function is not automatically invoked and must be by the API client.
         * @returns true if connection has been established, false if it has not.
         */
        public bool IsConnected { get; protected set; }

        protected abstract uint PrepareBuffer(BinaryWriter paramsList);

        protected void SendConnectRequest()
        {
            try
            {
                if (UseV100Plus)
                {
                    var paramsList = new BinaryWriter(new MemoryStream());

                    paramsList.AddParameter("API");

                    var lengthPos = PrepareBuffer(paramsList);

                    paramsList.Write(Encoding.ASCII.GetBytes(
                                         "v" +
                                         EncodedVersion +
                                         (IsEmpty(connectOptions) ? string.Empty : " " + connectOptions)));

                    CloseAndSend(paramsList, lengthPos);
                }
                else
                {
                    var buf = new List<byte>();

                    buf.AddRange(Encoding.UTF8.GetBytes(Constants.ClientVersion.ToString()));
                    buf.Add(Constants.Eol);
                    SocketTransport.Send(new EMessage(buf.ToArray()));
                }
            }
            catch (IOException)
            {
                Wrapper.Error(ClientId, EClientErrors.ConnectFail.Code, EClientErrors.ConnectFail.Message);
                throw;
            }
        }

        /**
         * @brief Initiates the message exchange between the client application and the TWS/IB Gateway
         */
        public void StartApi()
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 2;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.StartApi);
            paramsList.AddParameter(version);
            paramsList.AddParameter(ClientId);

            if (ServerVersion >= MinServerVer.OptionalCapabilities)
            {
                paramsList.AddParameter(OptionalCapabilities);
            }

            CloseAndSend(paramsList, lengthPos);
        }

        /**
         * @brief Terminates the connection and notifies the EWrapper implementing class.
         * @sa EWrapper::ConnectionClosed, eDisconnect
         */
        public void Close()
        {
            EDisconnect();
            Wrapper.ConnectionClosed();
        }

        /**
         * @brief Closes the socket connection and terminates its thread.
         */
        public virtual void EDisconnect(bool resetState = true)
        {
            if (SocketTransport == null)
            {
                return;
            }


            if (resetState)
            {
                IsConnected = false;
                ExtraAuth = false;
                ClientId = -1;
                ServerVersion = 0;
                OptionalCapabilities = "";
            }


            if (TcpStream != null)
            {
                TcpStream.Close();
            }

            if (resetState)
            {
                Wrapper.ConnectionClosed();
            }
        }

        /**
         * @brief Cancels tick-by-tick data.\n
         * @param reqId - unique identifier of the request.\n
         */
        public void CancelTickByTickData(int requestId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.TickByTick,
                                    " It does not support tick-by-tick cancels."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelTickByTickData);
            paramsList.AddParameter(requestId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCanceltickbytickdata);
        }

        /**
         * @brief Requests tick-by-tick data.\n
         * @param reqId - unique identifier of the request.\n
         * @param contract - the contract for which tick-by-tick data is requested.\n
         * @param tickType - tick-by-tick data type: "Last", "AllLast", "BidAsk" or "MidPoint".\n
         * @param numberOfTicks - number of ticks.\n
         * @param ignoreSize - ignore size flag.\n
         * @sa EWrapper::tickByTickAllLast, EWrapper::tickByTickBidAsk, EWrapper::tickByTickMidPoint, Contract
         */
        public void ReqTickByTickData(int requestId,
                                      Contract contract,
                                      string tickType,
                                      int numberOfTicks,
                                      bool ignoreSize)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.TickByTick,
                                    " It does not support tick-by-tick requests."))
            {
                return;
            }

            if ((numberOfTicks != 0 || ignoreSize) &&
                !CheckServerVersion(MinServerVer.TickByTickIgnoreSize,
                                    " It does not support ignoreSize and numberOfTicks parameters in tick-by-tick requests.")
            )
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqTickByTickData);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(contract.ConId);
            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);
            paramsList.AddParameter(contract.TradingClass);
            paramsList.AddParameter(tickType);

            if (ServerVersion >= MinServerVer.TickByTickIgnoreSize)
            {
                paramsList.AddParameter(numberOfTicks);
                paramsList.AddParameter(ignoreSize);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqtickbytickdata);
        }

        /**
        * @brief Cancels a historical data request.
        * @param reqId the request's identifier.
        * @sa reqHistoricalData
        */
        public void CancelHistoricalData(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(24, " It does not support historical data cancelations."))
            {
                return;
            }

            const int version = 1;
            //No server version validation takes place here since minimum is already higher
            SendCancelRequest(OutgoingMessages.CancelHistoricalData,
                              version,
                              reqId,
                              EClientErrors.FailSendCanhistdata);
        }

        /**
         * @brief Calculate the volatility for an option.\n
         * Request the calculation of the implied volatility based on hypothetical option and its underlying prices.\n The calculation will be return in EWrapper's tickOptionComputation callback.\n
         * @param reqId unique identifier of the request.\n
         * @param contract the option's contract for which the volatility wants to be calculated.\n
         * @param optionPrice hypothetical option price.\n
         * @param underPrice hypothetical option's underlying price.\n
         * @sa EWrapper::tickOptionComputation, cancelCalculateImpliedVolatility, Contract
         */
        public void CalculateImpliedVolatility(int reqId,
                                               Contract contract,
                                               double optionPrice,
                                               double underPrice,
                                               //reserved for future use, must be blank
                                               List<TagValue> impliedVolatilityOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqCalcImpliedVolat,
                                    " It does not support calculate implied volatility."))
            {
                return;
            }

            if (!Util.StringIsEmpty(contract.TradingClass) && !CheckServerVersion(MinServerVer.TradingClass, ""))
            {
                return;
            }

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqCalcImpliedVolat);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(contract.ConId);
            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            paramsList.AddParameter(optionPrice);
            paramsList.AddParameter(underPrice);

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(impliedVolatilityOptions);
            }

            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendReqcalcimpliedvolat);
        }

        /**
         * @brief Calculates an option's price based on the provided volatility and its underlying's price. \n
         * The calculation will be return in EWrapper's tickOptionComputation callback.\n
         * @param reqId request's unique identifier.\n
         * @param contract the option's contract for which the price wants to be calculated.\n
         * @param volatility hypothetical volatility.\n
         * @param underPrice hypothetical underlying's price.\n
         * @sa EWrapper::tickOptionComputation, cancelCalculateOptionPrice, Contract
         */
        public void CalculateOptionPrice(int reqId,
                                         Contract contract,
                                         double volatility,
                                         double underPrice,
                                         //reserved for future use, must be blank
                                         List<TagValue> optionPriceOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqCalcOptionPrice,
                                    " It does not support calculation price requests."))
            {
                return;
            }

            if (!Util.StringIsEmpty(contract.TradingClass) &&
                !CheckServerVersion(MinServerVer.ReqCalcOptionPrice,
                                    " It does not support tradingClass parameter in calculateOptionPrice."))
            {
                return;
            }

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqCalcOptionPrice);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(contract.ConId);
            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            paramsList.AddParameter(volatility);
            paramsList.AddParameter(underPrice);

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(optionPriceOptions);
            }

            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendReqcalcoptionprice);
        }

        /**
         * @brief Cancels the account's summary request.
         * After requesting an account's summary, invoke this function to cancel it.
         * @param reqId the identifier of the previously performed account request
         * @sa reqAccountSummary
         */
        public void CancelAccountSummary(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.AcctSummary,
                                    " It does not support account summary cancellation."))
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelAccountSummary, 1, reqId, EClientErrors.FailSendCanaccountdata);
        }

        /**
         * @brief Cancels an option's implied volatility calculation request
         * @param reqId the identifier of the implied volatility's calculation request.
         * @sa calculateImpliedVolatility
         */
        public void CancelCalculateImpliedVolatility(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.CancelCalcImpliedVolat,
                                    " It does not support calculate implied volatility cancellation."))
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelImpliedVolatility,
                              1,
                              reqId,
                              EClientErrors.FailSendCancalcimpliedvolat);
        }

        /**
         * @brief Cancels an option's price calculation request
         * @param reqId the identifier of the option's price's calculation request.
         * @sa calculateOptionPrice
         */
        public void CancelCalculateOptionPrice(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.CancelCalcOptionPrice,
                                    " It does not support calculate option price cancellation."))
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelOptionPrice, 1, reqId, EClientErrors.FailSendCancalcoptionprice);
        }

        /**
         * @brief Cancels Fundamental data request
         * @param reqId the request's identifier.
         * @sa reqFundamentalData
         */
        public void CancelFundamentalData(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.FundamentalData,
                                    " It does not support fundamental data requests."))
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelFundamentalData, 1, reqId, EClientErrors.FailSendCanfunddata);
        }


        /**
         * @brief Cancels a RT Market Data request
         * @param tickerId request's identifier
         * @sa reqMktData
         */
        public void CancelMktData(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelMarketData, 1, tickerId, EClientErrors.FailSendCanmkt);
        }

        /**
         * @brief Cancel's market depth's request.
         * @param tickerId request's identifier.
         * @sa reqMarketDepth
         */
        public void CancelMktDepth(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelMarketDepth,
                              1,
                              tickerId,
                              EClientErrors.FailSendCanmktdepth);
        }

        /**
         * @brief Cancels IB's news bulletin subscription
         * @sa reqNewsBulletins
         */
        public void CancelNewsBulletin()
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelNewsBulletin,
                              1,
                              EClientErrors.FailSendCorder);
        }

        /**
         * @brief Cancels an active order placed by from the same API client ID.\n
		 * Note: API clients cannot cancel individual orders placed by other clients. Only reqGlobalCancel is available.\n
         * @param orderId the order's client id
         * @sa placeOrder, reqGlobalCancel
         */
        public void CancelOrder(int orderId)
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelOrder,
                              1,
                              orderId,
                              EClientErrors.FailSendCorder);
        }

        /**
         * @brief Cancels a previous position subscription request made with reqPositions
         * @sa reqPositions
         */
        public void CancelPositions()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.AcctSummary,
                                    " It does not support position cancellation."))
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelPositions, 1, EClientErrors.FailSendCanpositions);
        }

        /**
         * @brief Cancels Real Time Bars' subscription
         * @param tickerId the request's identifier.
         * @sa reqRealTimeBars
         */
        public void CancelRealTimeBars(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelRealTimeBars, 1, tickerId, EClientErrors.FailSendCanrtbars);
        }

        /**
         * @brief Cancels Scanner Subscription
         * @param tickerId the subscription's unique identifier.
         * @sa reqScannerSubscription, ScannerSubscription, reqScannerParameters
         */
        public void CancelScannerSubscription(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            SendCancelRequest(OutgoingMessages.CancelScannerSubscription,
                              1,
                              tickerId,
                              EClientErrors.FailSendCanscanner);
        }

        /**
         * @brief Exercises an options contract\n
		 * Note: this function is affected by a TWS setting which specifies if an exercise request must be finalized
         * @param tickerId exercise request's identifier
         * @param contract the option Contract to be exercised.
         * @param exerciseAction set to 1 to exercise the option, set to 2 to let the option lapse.
         * @param exerciseQuantity number of contracts to be exercised
         * @param account destination account
         * @param ovrd Specifies whether your setting will override the system's natural action. For example, if your action is "exercise" and the option is not in-the-money, by natural action the option would not exercise. If you have override set to "yes" the natural action would be overridden and the out-of-the money option would be exercised. Set to 1 to override, set to 0 not to.
         */
        public void ExerciseOptions(int tickerId,
                                    Contract contract,
                                    int exerciseAction,
                                    int exerciseQuantity,
                                    string account,
                                    int ovrd)
        {
            //WARN needs to be tested!
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(21, " It does not support options exercise from the API."))
            {
                return;
            }

            if ((!Util.StringIsEmpty(contract.TradingClass) || contract.ConId > 0) &&
                !CheckServerVersion(MinServerVer.TradingClass,
                                    " It does not support conId not tradingClass parameter when exercising options."))
            {
                return;
            }

            var version = 2;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ExerciseOptions);
            paramsList.AddParameter(version);
            paramsList.AddParameter(tickerId);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);
            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            paramsList.AddParameter(exerciseAction);
            paramsList.AddParameter(exerciseQuantity);
            paramsList.AddParameter(account);
            paramsList.AddParameter(ovrd);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailGeneric);
        }

        /**
         * @brief Places or modifies an order
         * @param id the order's unique identifier. Use a sequential id starting with the id received at the nextValidId method. If a new order is placed with an order ID less than or equal to the order ID of a previous order an error will occur. 
         * @param contract the order's contract
         * @param order the order
         * @sa EWrapper::nextValidId, reqAllOpenOrders, reqAutoOpenOrders, reqOpenOrders, cancelOrder, reqGlobalCancel, EWrapper::openOrder, EWrapper::orderStatus, Order, Contract
         */
        public void PlaceOrder(int id, Contract contract, Order order)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!VerifyOrder(order, id, StringsAreEqual(Constants.BagSecType, contract.SecType)))
            {
                return;
            }

            if (!VerifyOrderContract(contract, id))
            {
                return;
            }

            var msgVersion = ServerVersion < MinServerVer.NotHeld ? 27 : 45;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);


            paramsList.AddParameter(OutgoingMessages.PlaceOrder);
            paramsList.AddParameter(msgVersion);
            paramsList.AddParameter(id);

            if (ServerVersion >= MinServerVer.PlaceOrderConid)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            if (ServerVersion >= 15)
            {
                paramsList.AddParameter(contract.Multiplier);
            }

            paramsList.AddParameter(contract.Exchange);
            if (ServerVersion >= 14)
            {
                paramsList.AddParameter(contract.PrimaryExch);
            }

            paramsList.AddParameter(contract.Currency);
            if (ServerVersion >= 2)
            {
                paramsList.AddParameter(contract.LocalSymbol);
            }

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            if (ServerVersion >= MinServerVer.SecIdType)
            {
                paramsList.AddParameter(contract.SecIdType);
                paramsList.AddParameter(contract.SecId);
            }

            // paramsList.AddParameter main order fields
            paramsList.AddParameter(order.Action);

            if (ServerVersion >= MinServerVer.FractionalPositions)
            {
                paramsList.AddParameter(order.TotalQuantity);
            }
            else
            {
                paramsList.AddParameter((int)order.TotalQuantity);
            }

            paramsList.AddParameter(order.OrderType);
            if (ServerVersion < MinServerVer.OrderComboLegsPrice)
            {
                paramsList.AddParameter(order.LmtPrice == double.MaxValue ? 0 : order.LmtPrice);
            }
            else
            {
                paramsList.AddParameterMax(order.LmtPrice);
            }

            if (ServerVersion < MinServerVer.TrailingPercent)
            {
                paramsList.AddParameter(order.AuxPrice == double.MaxValue ? 0 : order.AuxPrice);
            }
            else
            {
                paramsList.AddParameterMax(order.AuxPrice);
            }

            // paramsList.AddParameter extended order fields
            paramsList.AddParameter(order.Tif);
            paramsList.AddParameter(order.OcaGroup);
            paramsList.AddParameter(order.Account);
            paramsList.AddParameter(order.OpenClose);
            paramsList.AddParameter(order.Origin);
            paramsList.AddParameter(order.OrderRef);
            paramsList.AddParameter(order.Transmit);
            if (ServerVersion >= 4)
            {
                paramsList.AddParameter(order.ParentId);
            }

            if (ServerVersion >= 5)
            {
                paramsList.AddParameter(order.BlockOrder);
                paramsList.AddParameter(order.SweepToFill);
                paramsList.AddParameter(order.DisplaySize);
                paramsList.AddParameter(order.TriggerMethod);
                if (ServerVersion < 38)
                {
                    // will never happen
                    paramsList.AddParameter( /* order.ignoreRth */ false);
                }
                else
                {
                    paramsList.AddParameter(order.OutsideRth);
                }
            }

            if (ServerVersion >= 7)
            {
                paramsList.AddParameter(order.Hidden);
            }

            // paramsList.AddParameter combo legs for BAG requests
            var isBag = StringsAreEqual(Constants.BagSecType, contract.SecType);
            if (ServerVersion >= 8 && isBag)
            {
                if (contract.ComboLegs == null)
                {
                    paramsList.AddParameter(0);
                }
                else
                {
                    paramsList.AddParameter(contract.ComboLegs.Count);

                    ComboLeg comboLeg;
                    for (var i = 0; i < contract.ComboLegs.Count; i++)
                    {
                        comboLeg = contract.ComboLegs[i];
                        paramsList.AddParameter(comboLeg.ConId);
                        paramsList.AddParameter(comboLeg.Ratio);
                        paramsList.AddParameter(comboLeg.Action);
                        paramsList.AddParameter(comboLeg.Exchange);
                        paramsList.AddParameter(comboLeg.OpenClose);

                        if (ServerVersion >= MinServerVer.SshortComboLegs)
                        {
                            paramsList.AddParameter(comboLeg.ShortSaleSlot);
                            paramsList.AddParameter(comboLeg.DesignatedLocation);
                        }

                        if (ServerVersion >= MinServerVer.SshortxOld)
                        {
                            paramsList.AddParameter(comboLeg.ExemptCode);
                        }
                    }
                }
            }

            // add order combo legs for BAG requests
            if (ServerVersion >= MinServerVer.OrderComboLegsPrice && isBag)
            {
                if (order.OrderComboLegs == null)
                {
                    paramsList.AddParameter(0);
                }
                else
                {
                    paramsList.AddParameter(order.OrderComboLegs.Count);

                    for (var i = 0; i < order.OrderComboLegs.Count; i++)
                    {
                        var orderComboLeg = order.OrderComboLegs[i];
                        paramsList.AddParameterMax(orderComboLeg.Price);
                    }
                }
            }

            if (ServerVersion >= MinServerVer.SmartComboRoutingParams && isBag)
            {
                var smartComboRoutingParams = order.SmartComboRoutingParams;
                var smartComboRoutingParamsCount = smartComboRoutingParams == null ? 0 : smartComboRoutingParams.Count;
                paramsList.AddParameter(smartComboRoutingParamsCount);
                if (smartComboRoutingParamsCount > 0)
                {
                    for (var i = 0; i < smartComboRoutingParamsCount; ++i)
                    {
                        var tagValue = smartComboRoutingParams[i];
                        paramsList.AddParameter(tagValue.Tag);
                        paramsList.AddParameter(tagValue.Value);
                    }
                }
            }

            if (ServerVersion >= 9)
            {
                // paramsList.AddParameter deprecated sharesAllocation field
                paramsList.AddParameter("");
            }

            if (ServerVersion >= 10)
            {
                paramsList.AddParameter(order.DiscretionaryAmt);
            }

            if (ServerVersion >= 11)
            {
                paramsList.AddParameter(order.GoodAfterTime);
            }

            if (ServerVersion >= 12)
            {
                paramsList.AddParameter(order.GoodTillDate);
            }

            if (ServerVersion >= 13)
            {
                paramsList.AddParameter(order.FaGroup);
                paramsList.AddParameter(order.FaMethod);
                paramsList.AddParameter(order.FaPercentage);
                paramsList.AddParameter(order.FaProfile);
            }

            if (ServerVersion >= MinServerVer.ModelsSupport)
            {
                paramsList.AddParameter(order.ModelCode);
            }

            if (ServerVersion >= 18)
            {
                // institutional short sale slot fields.
                paramsList.AddParameter(order.ShortSaleSlot); // 0 only for retail, 1 or 2 only for institution.
                paramsList.AddParameter(order.DesignatedLocation); // only populate when order.shortSaleSlot = 2.
            }

            if (ServerVersion >= MinServerVer.SshortxOld)
            {
                paramsList.AddParameter(order.ExemptCode);
            }

            if (ServerVersion >= 19)
            {
                paramsList.AddParameter(order.OcaType);
                if (ServerVersion < 38)
                {
                    // will never happen
                    paramsList.AddParameter( /* order.rthOnly */ false);
                }

                paramsList.AddParameter(order.Rule80A);
                paramsList.AddParameter(order.SettlingFirm);
                paramsList.AddParameter(order.AllOrNone);
                paramsList.AddParameterMax(order.MinQty);
                paramsList.AddParameterMax(order.PercentOffset);
                paramsList.AddParameter(order.ETradeOnly);
                paramsList.AddParameter(order.FirmQuoteOnly);
                paramsList.AddParameterMax(order.NbboPriceCap);
                paramsList.AddParameterMax(order.AuctionStrategy);
                paramsList.AddParameterMax(order.StartingPrice);
                paramsList.AddParameterMax(order.StockRefPrice);
                paramsList.AddParameterMax(order.Delta);
                // Volatility orders had specific watermark price attribs in server version 26
                var lower = ServerVersion == 26 && order.OrderType.Equals("VOL")
                    ? double.MaxValue
                    : order.StockRangeLower;
                var upper = ServerVersion == 26 && order.OrderType.Equals("VOL")
                    ? double.MaxValue
                    : order.StockRangeUpper;
                paramsList.AddParameterMax(lower);
                paramsList.AddParameterMax(upper);
            }

            if (ServerVersion >= 22)
            {
                paramsList.AddParameter(order.OverridePercentageConstraints);
            }

            if (ServerVersion >= 26)
            {
                // Volatility orders
                paramsList.AddParameterMax(order.Volatility);
                paramsList.AddParameterMax(order.VolatilityType);
                if (ServerVersion < 28)
                {
                    var isDeltaNeutralTypeMkt = string.Compare("MKT", order.DeltaNeutralOrderType, true) == 0;
                    paramsList.AddParameter(isDeltaNeutralTypeMkt);
                }
                else
                {
                    paramsList.AddParameter(order.DeltaNeutralOrderType);
                    paramsList.AddParameterMax(order.DeltaNeutralAuxPrice);

                    if (ServerVersion >= MinServerVer.DeltaNeutralConid && !IsEmpty(order.DeltaNeutralOrderType))
                    {
                        paramsList.AddParameter(order.DeltaNeutralConId);
                        paramsList.AddParameter(order.DeltaNeutralSettlingFirm);
                        paramsList.AddParameter(order.DeltaNeutralClearingAccount);
                        paramsList.AddParameter(order.DeltaNeutralClearingIntent);
                    }

                    if (ServerVersion >= MinServerVer.DeltaNeutralOpenClose && !IsEmpty(order.DeltaNeutralOrderType))
                    {
                        paramsList.AddParameter(order.DeltaNeutralOpenClose);
                        paramsList.AddParameter(order.DeltaNeutralShortSale);
                        paramsList.AddParameter(order.DeltaNeutralShortSaleSlot);
                        paramsList.AddParameter(order.DeltaNeutralDesignatedLocation);
                    }
                }

                paramsList.AddParameter(order.ContinuousUpdate);
                if (ServerVersion == 26)
                {
                    // Volatility orders had specific watermark price attribs in server version 26
                    var lower = order.OrderType.Equals("VOL") ? order.StockRangeLower : double.MaxValue;
                    var upper = order.OrderType.Equals("VOL") ? order.StockRangeUpper : double.MaxValue;
                    paramsList.AddParameterMax(lower);
                    paramsList.AddParameterMax(upper);
                }

                paramsList.AddParameterMax(order.ReferencePriceType);
            }

            if (ServerVersion >= 30)
            {
                // TRAIL_STOP_LIMIT stop price
                paramsList.AddParameterMax(order.TrailStopPrice);
            }

            if (ServerVersion >= MinServerVer.TrailingPercent)
            {
                paramsList.AddParameterMax(order.TrailingPercent);
            }

            if (ServerVersion >= MinServerVer.ScaleOrders)
            {
                if (ServerVersion >= MinServerVer.ScaleOrders2)
                {
                    paramsList.AddParameterMax(order.ScaleInitLevelSize);
                    paramsList.AddParameterMax(order.ScaleSubsLevelSize);
                }
                else
                {
                    paramsList.AddParameter("");
                    paramsList.AddParameterMax(order.ScaleInitLevelSize);
                }

                paramsList.AddParameterMax(order.ScalePriceIncrement);
            }

            if (ServerVersion >= MinServerVer.ScaleOrders3 &&
                order.ScalePriceIncrement > 0.0 &&
                order.ScalePriceIncrement != double.MaxValue)
            {
                paramsList.AddParameterMax(order.ScalePriceAdjustValue);
                paramsList.AddParameterMax(order.ScalePriceAdjustInterval);
                paramsList.AddParameterMax(order.ScaleProfitOffset);
                paramsList.AddParameter(order.ScaleAutoReset);
                paramsList.AddParameterMax(order.ScaleInitPosition);
                paramsList.AddParameterMax(order.ScaleInitFillQty);
                paramsList.AddParameter(order.ScaleRandomPercent);
            }

            if (ServerVersion >= MinServerVer.ScaleTable)
            {
                paramsList.AddParameter(order.ScaleTable);
                paramsList.AddParameter(order.ActiveStartTime);
                paramsList.AddParameter(order.ActiveStopTime);
            }

            if (ServerVersion >= MinServerVer.HedgeOrders)
            {
                paramsList.AddParameter(order.HedgeType);
                if (!IsEmpty(order.HedgeType))
                {
                    paramsList.AddParameter(order.HedgeParam);
                }
            }

            if (ServerVersion >= MinServerVer.OptOutSmartRouting)
            {
                paramsList.AddParameter(order.OptOutSmartRouting);
            }

            if (ServerVersion >= MinServerVer.PtaOrders)
            {
                paramsList.AddParameter(order.ClearingAccount);
                paramsList.AddParameter(order.ClearingIntent);
            }

            if (ServerVersion >= MinServerVer.NotHeld)
            {
                paramsList.AddParameter(order.NotHeld);
            }

            if (ServerVersion >= MinServerVer.DeltaNeutral)
            {
                if (contract.DeltaNeutralContract != null)
                {
                    var deltaNeutralContract = contract.DeltaNeutralContract;
                    paramsList.AddParameter(true);
                    paramsList.AddParameter(deltaNeutralContract.ConId);
                    paramsList.AddParameter(deltaNeutralContract.Delta);
                    paramsList.AddParameter(deltaNeutralContract.Price);
                }
                else
                {
                    paramsList.AddParameter(false);
                }
            }

            if (ServerVersion >= MinServerVer.AlgoOrders)
            {
                paramsList.AddParameter(order.AlgoStrategy);
                if (!IsEmpty(order.AlgoStrategy))
                {
                    var algoParams = order.AlgoParams;
                    var algoParamsCount = algoParams == null ? 0 : algoParams.Count;
                    paramsList.AddParameter(algoParamsCount);
                    if (algoParamsCount > 0)
                    {
                        for (var i = 0; i < algoParamsCount; ++i)
                        {
                            var tagValue = algoParams[i];
                            paramsList.AddParameter(tagValue.Tag);
                            paramsList.AddParameter(tagValue.Value);
                        }
                    }
                }
            }

            if (ServerVersion >= MinServerVer.AlgoId)
            {
                paramsList.AddParameter(order.AlgoId);
            }

            if (ServerVersion >= MinServerVer.WhatIfOrders)
            {
                paramsList.AddParameter(order.WhatIf);
            }

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(order.OrderMiscOptions);
            }

            if (ServerVersion >= MinServerVer.OrderSolicited)
            {
                paramsList.AddParameter(order.Solicited);
            }

            if (ServerVersion >= MinServerVer.RandomizeSizeAndPrice)
            {
                paramsList.AddParameter(order.RandomizeSize);
                paramsList.AddParameter(order.RandomizePrice);
            }

            if (ServerVersion >= MinServerVer.PeggedToBenchmark)
            {
                if (order.OrderType == "PEG BENCH")
                {
                    paramsList.AddParameter(order.ReferenceContractId);
                    paramsList.AddParameter(order.IsPeggedChangeAmountDecrease);
                    paramsList.AddParameter(order.PeggedChangeAmount);
                    paramsList.AddParameter(order.ReferenceChangeAmount);
                    paramsList.AddParameter(order.ReferenceExchange);
                }

                paramsList.AddParameter(order.Conditions.Count);

                if (order.Conditions.Count > 0)
                {
                    foreach (var item in order.Conditions)
                    {
                        paramsList.AddParameter((int)item.Type);
                        item.Serialize(paramsList);
                    }

                    paramsList.AddParameter(order.ConditionsIgnoreRth);
                    paramsList.AddParameter(order.ConditionsCancelOrder);
                }

                paramsList.AddParameter(order.AdjustedOrderType);
                paramsList.AddParameter(order.TriggerPrice);
                paramsList.AddParameter(order.LmtPriceOffset);
                paramsList.AddParameter(order.AdjustedStopPrice);
                paramsList.AddParameter(order.AdjustedStopLimitPrice);
                paramsList.AddParameter(order.AdjustedTrailingAmount);
                paramsList.AddParameter(order.AdjustableTrailingUnit);
            }

            if (ServerVersion >= MinServerVer.ExtOperator)
            {
                paramsList.AddParameter(order.ExtOperator);
            }

            if (ServerVersion >= MinServerVer.SoftDollarTier)
            {
                paramsList.AddParameter(order.Tier.Name);
                paramsList.AddParameter(order.Tier.Value);
            }

            if (ServerVersion >= MinServerVer.CashQty)
            {
                paramsList.AddParameterMax(order.CashQty);
            }

            if (ServerVersion >= MinServerVer.DecisionMaker)
            {
                paramsList.AddParameter(order.Mifid2DecisionMaker);
                paramsList.AddParameter(order.Mifid2DecisionAlgo);
            }

            if (ServerVersion >= MinServerVer.MifidExecution)
            {
                paramsList.AddParameter(order.Mifid2ExecutionTrader);
                paramsList.AddParameter(order.Mifid2ExecutionAlgo);
            }

            if (ServerVersion >= MinServerVer.AutoPriceForHedge)
            {
                paramsList.AddParameter(order.DontUseAutoPriceForHedge);
            }

            CloseAndSend(id, paramsList, lengthPos, EClientErrors.FailSendOrder);
        }

        /**
         * @brief Replaces Financial Advisor's settings
         * A Financial Advisor can define three different configurations: 
         *    1. Groups: offer traders a way to create a group of accounts and apply a single allocation method to all accounts in the group.
         *    2. Profiles: let you allocate shares on an account-by-account basis using a predefined calculation value.
         *    3. Account Aliases: let you easily identify the accounts by meaningful names rather than account numbers.
         * More information at https://www.interactivebrokers.com/en/?f=%2Fen%2Fsoftware%2Fpdfhighlights%2FPDF-AdvisorAllocations.php%3Fib_entity%3Dllc
         * @param faDataType the configuration to change. Set to 1, 2 or 3 as defined above.
         * @param xml the xml-formatted configuration string
         * @sa requestFA 
         */
        public void ReplaceFa(int faDataType, string xml)
        {
            if (!CheckConnection())
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReplaceFa);
            paramsList.AddParameter(1);
            paramsList.AddParameter(faDataType);
            paramsList.AddParameter(xml);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendFaReplace);
        }

        /**
         * @brief Requests the FA configuration
         * A Financial Advisor can define three different configurations: 
         *      1. Groups: offer traders a way to create a group of accounts and apply a single allocation method to all accounts in the group.
         *      2. Profiles: let you allocate shares on an account-by-account basis using a predefined calculation value.
         *      3. Account Aliases: let you easily identify the accounts by meaningful names rather than account numbers.
         * More information at https://www.interactivebrokers.com/en/?f=%2Fen%2Fsoftware%2Fpdfhighlights%2FPDF-AdvisorAllocations.php%3Fib_entity%3Dllc
         * @param faDataType the configuration to change. Set to 1, 2 or 3 as defined above.
         * @sa replaceFA 
         */
        public void RequestFa(int faDataType)
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestFa);
            paramsList.AddParameter(version);
            paramsList.AddParameter(faDataType);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendFaRequest);
        }

        /**
         * @brief Requests a specific account's summary.\n
         * This method will subscribe to the account summary as presented in the TWS' Account Summary tab. The data is returned at EWrapper::accountSummary\n
         * https://www.interactivebrokers.com/en/software/tws/accountwindowtop.htm
         * @param reqId the unique request identifier.\n
         * @param group set to "All" to return account summary data for all accounts, or set to a specific Advisor Account Group name that has already been created in TWS Global Configuration.\n
         * @param tags a comma separated list with the desired tags:
         *      - AccountType — Identifies the IB account structure
         *      - NetLiquidation — The basis for determining the price of the assets in your account. Total cash value + stock value + options value + bond value
         *      - TotalCashValue — Total cash balance recognized at the time of trade + futures PNL
         *      - SettledCash — Cash recognized at the time of settlement - purchases at the time of trade - commissions - taxes - fees
         *      - AccruedCash — Total accrued cash value of stock, commodities and securities
         *      - BuyingPower — Buying power serves as a measurement of the dollar value of securities that one may purchase in a securities account without depositing additional funds
         *      - EquityWithLoanValue — Forms the basis for determining whether a client has the necessary assets to either initiate or maintain security positions. Cash + stocks + bonds + mutual funds
         *      - PreviousEquityWithLoanValue — Marginable Equity with Loan value as of 16:00 ET the previous day
         *      - GrossPositionValue — The sum of the absolute value of all stock and equity option positions
         *      - RegTEquity — Regulation T equity for universal account
         *      - RegTMargin — Regulation T margin for universal account
         *      - SMA — Special Memorandum Account: Line of credit created when the market value of securities in a Regulation T account increase in value
         *      - InitMarginReq — Initial Margin requirement of whole portfolio
         *      - MaintMarginReq — Maintenance Margin requirement of whole portfolio
         *      - AvailableFunds — This value tells what you have available for trading
         *      - ExcessLiquidity — This value shows your margin cushion, before liquidation
         *      - Cushion — Excess liquidity as a percentage of net liquidation value
         *      - FullInitMarginReq — Initial Margin of whole portfolio with no discounts or intraday credits
         *      - FullMaintMarginReq — Maintenance Margin of whole portfolio with no discounts or intraday credits
         *      - FullAvailableFunds — Available funds of whole portfolio with no discounts or intraday credits
         *      - FullExcessLiquidity — Excess liquidity of whole portfolio with no discounts or intraday credits
         *      - LookAheadNextChange — Time when look-ahead values take effect
         *      - LookAheadInitMarginReq — Initial Margin requirement of whole portfolio as of next period's margin change
         *      - LookAheadMaintMarginReq — Maintenance Margin requirement of whole portfolio as of next period's margin change
         *      - LookAheadAvailableFunds — This value reflects your available funds at the next margin change
         *      - LookAheadExcessLiquidity — This value reflects your excess liquidity at the next margin change
         *      - HighestSeverity — A measure of how close the account is to liquidation
         *      - DayTradesRemaining — The Number of Open/Close trades a user could put on before Pattern Day Trading is detected. A value of "-1" means that the user can put on unlimited day trades.
         *      - Leverage — GrossPositionValue / NetLiquidation
         *      - $LEDGER — Single flag to relay all cash balance tags*, only in base currency.
         *      - $LEDGER:CURRENCY — Single flag to relay all cash balance tags*, only in the specified currency.
         *      - $LEDGER:ALL — Single flag to relay all cash balance tags* in all currencies.
         * @sa cancelAccountSummary, EWrapper::accountSummary, EWrapper::accountSummaryEnd
         */
        public void ReqAccountSummary(int reqId, string group, string tags)
        {
            var version = 1;
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(reqId,
                                    MinServerVer.AcctSummary,
                                    " It does not support account summary requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAccountSummary);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(group);
            paramsList.AddParameter(tags);
            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendReqaccountdata);
        }

        /**
         * @brief Subscribes to an specific account's information and portfolio
         * Through this method, a single account's subscription can be started/stopped. As a result from the subscription, the account's information, portfolio and last update time will be received at EWrapper::updateAccountValue, EWrapper::updateAccountPortfolio, EWrapper::updateAccountTime respectively. All account values and positions will be returned initially, and then there will only be updates when there is a change in a position, or to an account value every 3 minutes if it has changed. 
         * Only one account can be subscribed at a time. A second subscription request for another account when the previous one is still active will cause the first one to be canceled in favour of the second one. Consider user reqPositions if you want to retrieve all your accounts' portfolios directly.
         * @param subscribe set to true to start the subscription and to false to stop it.
         * @param acctCode the account id (i.e. U123456) for which the information is requested.
         * @sa reqPositions, EWrapper::updateAccountValue, EWrapper::updatePortfolio, EWrapper::updateAccountTime
         */
        public void ReqAccountUpdates(bool subscribe, string acctCode)
        {
            var version = 2;
            if (!CheckConnection())
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAccountData);
            paramsList.AddParameter(version);
            paramsList.AddParameter(subscribe);
            if (ServerVersion >= 9)
            {
                paramsList.AddParameter(acctCode);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqaccountdata);
        }

        /**
         * @brief Requests all *current* open orders in associated accounts at the current moment. The existing orders will be received via the openOrder and orderStatus events.
		 * Open orders are returned once; this function does not initiate a subscription
		 * @sa reqAutoOpenOrders, reqOpenOrders, EWrapper::openOrder, EWrapper::orderStatus, EWrapper::openOrderEnd
         */
        public void ReqAllOpenOrders()
        {
            var version = 1;
            if (!CheckConnection())
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAllOpenOrders);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendOorder);
        }

        /**
         * @brief Requests status updates about future orders placed from TWS. Can only be used with client ID 0. 
         * @param autoBind if set to true, the newly created orders will be assigned an API order ID and implicitly associated with this client. If set to false, future orders will not be.
         * @sa reqAllOpenOrders, reqOpenOrders, cancelOrder, reqGlobalCancel, EWrapper::openOrder, EWrapper::orderStatus
         */
        public void ReqAutoOpenOrders(bool autoBind)
        {
            var version = 1;
            if (!CheckConnection())
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAutoOpenOrders);
            paramsList.AddParameter(version);
            paramsList.AddParameter(autoBind);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendOorder);
        }

        /**
         * @brief Requests contract information.\n
         * This method will provide all the contracts matching the contract provided. It can also be used to retrieve complete options and futures chains. This information will be returned at EWrapper:contractDetails. Though it is now (in API version > 9.72.12) advised to use reqSecDefOptParams for that purpose. \n
         * @param reqId the unique request identifier.\n
         * @param contract the contract used as sample to query the available contracts. Typically, it will contain the Contract::Symbol, Contract::Currency, Contract::SecType, Contract::Exchange\n
         * @sa EWrapper::contractDetails, EWrapper::contractDetailsEnd
         */
        public void ReqContractDetails(int reqId, Contract contract)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!IsEmpty(contract.SecIdType) || !IsEmpty(contract.SecId))
            {
                if (!CheckServerVersion(reqId,
                                        MinServerVer.SecIdType,
                                        " It does not support secIdType not secId attributes"))
                {
                    return;
                }
            }

            if (!IsEmpty(contract.TradingClass))
            {
                if (!CheckServerVersion(reqId,
                                        MinServerVer.TradingClass,
                                        " It does not support the TradingClass parameter when requesting contract details.")
                )
                {
                    return;
                }
            }

            if (!IsEmpty(contract.PrimaryExch) &&
                !CheckServerVersion(reqId,
                                    MinServerVer.Linking,
                                    " It does not support PrimaryExch parameter when requesting contract details."))
            {
                return;
            }


            var version = 8;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestContractData);
            paramsList.AddParameter(version); //version
            if (ServerVersion >= MinServerVer.ContractDataChain)
            {
                paramsList.AddParameter(reqId);
            }

            if (ServerVersion >= MinServerVer.ContractConid)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            if (ServerVersion >= 15)
            {
                paramsList.AddParameter(contract.Multiplier);
            }

            if (ServerVersion >= MinServerVer.Primaryexch)
            {
                paramsList.AddParameter(contract.Exchange);
                paramsList.AddParameter(contract.PrimaryExch);
            }
            else if (ServerVersion >= MinServerVer.Linking)
            {
                if (!IsEmpty(contract.PrimaryExch) && (contract.Exchange == "BEST" || contract.Exchange == "SMART"))
                {
                    paramsList.AddParameter(contract.Exchange + ":" + contract.PrimaryExch);
                }
                else
                {
                    paramsList.AddParameter(contract.Exchange);
                }
            }

            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);
            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            if (ServerVersion >= 31)
            {
                paramsList.AddParameter(contract.IncludeExpired);
            }

            if (ServerVersion >= MinServerVer.SecIdType)
            {
                paramsList.AddParameter(contract.SecIdType);
                paramsList.AddParameter(contract.SecId);
            }

            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendReqcontract);
        }

        /**
         * @brief Requests TWS's current time.
         * @sa EWrapper::currentTime
         */
        public void ReqCurrentTime()
        {
            var version = 1;
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.CurrentTime, " It does not support current time requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestCurrentTime);
            paramsList.AddParameter(version); //version
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqcurrtime);
        }

        /**
         * @brief Requests current day's (since midnight) executions matching the filter.
         * Only the current day's executions can be retrieved. Along with the executions, the CommissionReport will also be returned. The execution details will arrive at EWrapper:execDetails
         * @param reqId the request's unique identifier.
         * @param filter the filter criteria used to determine which execution reports are returned.
         * @sa EWrapper::execDetails, EWrapper::commissionReport, ExecutionFilter
         */
        public void ReqExecutions(int reqId, ExecutionFilter filter)
        {
            if (!CheckConnection())
            {
                return;
            }

            var version = 3;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestExecutions);
            paramsList.AddParameter(version); //version

            if (ServerVersion >= MinServerVer.ExecutionDataChain)
            {
                paramsList.AddParameter(reqId);
            }

            //Send the execution rpt filter data
            if (ServerVersion >= 9)
            {
                paramsList.AddParameter(filter.ClientId);
                paramsList.AddParameter(filter.AcctCode);

                // Note that the valid format for time is "yyyymmdd-hh:mm:ss"
                paramsList.AddParameter(filter.Time);
                paramsList.AddParameter(filter.Symbol);
                paramsList.AddParameter(filter.SecType);
                paramsList.AddParameter(filter.Exchange);
                paramsList.AddParameter(filter.Side);
            }

            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendExec);
        }

        /**
         * @brief Requests the contract's Reuters or Wall Street Horizons fundamental data.
         * Fundalmental data is returned at EWrapper::fundamentalData
         * @param reqId the request's unique identifier.
         * @param contract the contract's description for which the data will be returned.
         * @param reportType there are three available report types: 
         *      - ReportSnapshot: Company overview
         *      - ReportsFinSummary: Financial summary
                - ReportRatios:	Financial ratios
                - ReportsFinStatements:	Financial statements
                - RESC: Analyst estimates
                - CalendarReport: Company calendar from Wall Street Horizons
         * @sa EWrapper::fundamentalData
         */
        public void ReqFundamentalData(int reqId,
                                       Contract contract,
                                       string reportType,
                                       //reserved for future use, must be blank
                                       List<TagValue> fundamentalDataOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(reqId,
                                    MinServerVer.FundamentalData,
                                    " It does not support Fundamental Data requests."))
            {
                return;
            }

            if (!IsEmpty(contract.TradingClass) || contract.ConId > 0 || !IsEmpty(contract.Multiplier))
            {
                if (!CheckServerVersion(reqId, MinServerVer.TradingClass, ""))
                {
                    return;
                }
            }

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestFundamentalData);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                //WARN: why are we checking the trading class and multiplier above never send them?
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);
            paramsList.AddParameter(reportType);

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(fundamentalDataOptions);
            }

            CloseAndSend(reqId, paramsList, lengthPos, EClientErrors.FailSendReqfunddata);
        }

        /**
         * @brief Cancels all active orders.\n
         * This method will cancel ALL open orders including those placed directly from TWS.
         * @sa cancelOrder
         */
        public void ReqGlobalCancel()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqGlobalCancel, "It does not support global cancel requests."))
            {
                return;
            }

            const int version = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestGlobalCancel);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqglobalcancel);
        }

        /**
         * @brief Requests contracts' historical data.
         * When requesting historical data, a finishing time and date is required along with a duration string. For example, having: 
         *      - endDateTime: 20130701 23:59:59 GMT
         *      - durationStr: 3 D
         * will return three days of data counting backwards from July 1st 2013 at 23:59:59 GMT resulting in all the available bars of the last three days until the date and time specified. It is possible to specify a timezone optionally. The resulting bars will be returned in EWrapper::historicalData
         * @param tickerId the request's unique identifier.
         * @param contract the contract for which we want to retrieve the data.
         * @param endDateTime request's ending time with format yyyyMMdd HH:mm:ss {TMZ}
         * @param durationString the amount of time for which the data needs to be retrieved:
         *      - " S (seconds)
         *      - " D (days)
         *      - " W (weeks)
         *      - " M (months)
         *      - " Y (years)
         * @param barSizeSetting the size of the bar:
         *      - 1 sec
         *      - 5 secs
         *      - 15 secs
         *      - 30 secs
         *      - 1 min
         *      - 2 mins
         *      - 3 mins
         *      - 5 mins
         *      - 15 mins
         *      - 30 mins
         *      - 1 hour
         *      - 1 day
         * @param whatToShow the kind of information being retrieved:
         *      - TRADES
         *      - MIDPOINT
         *      - BID
         *      - ASK
         *      - BID_ASK
         *      - HISTORICAL_VOLATILITY
         *      - OPTION_IMPLIED_VOLATILITY
         *	    - FEE_RATE
         *	    - REBATE_RATE
         * @param useRTH set to 0 to obtain the data which was also generated outside of the Regular Trading Hours, set to 1 to obtain only the RTH data
         * @param formatDate set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds
		 * @param keepUpToDate set to True to received continuous updates on most recent bar data. If True, and endDateTime cannot be specified.
         * @sa EWrapper::historicalData
         */
        public void ReqHistoricalData(int tickerId,
                                      Contract contract,
                                      string endDateTime,
                                      string durationString,
                                      string barSizeSetting,
                                      string whatToShow,
                                      int useRth,
                                      int formatDate,
                                      bool keepUpToDate,
                                      List<TagValue> chartOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(tickerId, 16))
            {
                return;
            }

            if (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
            {
                if (!CheckServerVersion(tickerId,
                                        MinServerVer.TradingClass,
                                        " It does not support conId nor trading class parameters when requesting historical data.")
                )
                {
                    return;
                }
            }

            const int version = 6;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestHistoricalData);

            if (ServerVersion < MinServerVer.SyntRealtimeBars)
            {
                paramsList.AddParameter(version);
            }

            paramsList.AddParameter(tickerId);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            paramsList.AddParameter(contract.IncludeExpired ? 1 : 0);


            paramsList.AddParameter(endDateTime);
            paramsList.AddParameter(barSizeSetting);

            paramsList.AddParameter(durationString);
            paramsList.AddParameter(useRth);
            paramsList.AddParameter(whatToShow);

            paramsList.AddParameter(formatDate);

            if (StringsAreEqual(Constants.BagSecType, contract.SecType))
            {
                if (contract.ComboLegs == null)
                {
                    paramsList.AddParameter(0);
                }
                else
                {
                    paramsList.AddParameter(contract.ComboLegs.Count);

                    ComboLeg comboLeg;
                    for (var i = 0; i < contract.ComboLegs.Count; i++)
                    {
                        comboLeg = contract.ComboLegs[i];
                        paramsList.AddParameter(comboLeg.ConId);
                        paramsList.AddParameter(comboLeg.Ratio);
                        paramsList.AddParameter(comboLeg.Action);
                        paramsList.AddParameter(comboLeg.Exchange);
                    }
                }
            }

            if (ServerVersion >= MinServerVer.SyntRealtimeBars)
            {
                paramsList.AddParameter(keepUpToDate);
            }

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(chartOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqhistdata);
        }

        /**
         * @brief Requests the next valid order ID at the current moment.
         * @param numIds deprecated- this parameter will not affect the value returned to nextValidId
         * @sa EWrapper::nextValidId
         */
        public void ReqIds(int numIds)
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestIds);
            paramsList.AddParameter(version);
            paramsList.AddParameter(numIds);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailGeneric);
        }

        /**
         * @brief Requests the accounts to which the logged user has access to.
         * @sa EWrapper::managedAccounts
         */
        public void ReqManagedAccts()
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestManagedAccounts);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailGeneric);
        }

        /**
         * @brief Requests real time market data.
         * Returns market data for an instrument either in real time or 10-15 minutes delayed (depending on the market data type specified)
         * @param tickerId the request's identifier
         * @param contract the Contract for which the data is being requested
         * @param genericTickList comma separated ids of the available generic ticks:
         *      - 100 	Option Volume (currently for stocks)
         *      - 101 	Option Open Interest (currently for stocks) 
         *      - 104 	Historical Volatility (currently for stocks)
         *      - 105 	Average Option Volume (currently for stocks)
         *      - 106 	Option Implied Volatility (currently for stocks)
         *      - 162 	Index Future Premium 
         *      - 165 	Miscellaneous Stats 
         *      - 221 	Mark Price (used in TWS P&L computations) 
         *      - 225 	Auction values (volume, price and imbalance) 
         *      - 233 	RTVolume - contains the last trade price, last trade size, last trade time, total volume, VWAP, and single trade flag.
         *      - 236 	Shortable
         *      - 256 	Inventory 	 
         *      - 258 	Fundamental Ratios 
         *      - 411 	Realtime Historical Volatility 
         *      - 456 	IBDividends
         * @param snapshot for users with corresponding real time market data subscriptions. A true value will return a one-time snapshot, while a false value will provide streaming data. 
     * @param regulatory snapshot for US stocks requests NBBO snapshots for users which have "US Securities Snapshot Bundle" subscription but not corresponding Network A, B, or C subscription necessary for streaming 		 * market data. One-time snapshot of current market price that will incur a fee of 1 cent to the account per snapshot. 
         * @sa cancelMktData, EWrapper::tickPrice, EWrapper::tickSize, EWrapper::tickString, EWrapper::tickEFP, EWrapper::tickGeneric, EWrapper::tickOptionComputation, EWrapper::tickSnapshotEnd
         */
        public void ReqMktData(int tickerId,
                               Contract contract,
                               string genericTickList,
                               bool snapshot,
                               bool regulatorySnaphsot,
                               List<TagValue> mktDataOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (snapshot &&
                !CheckServerVersion(tickerId,
                                    MinServerVer.SnapshotMktData,
                                    "It does not support snapshot market data requests."))
            {
                return;
            }

            if (contract.DeltaNeutralContract != null &&
                !CheckServerVersion(tickerId,
                                    MinServerVer.DeltaNeutral,
                                    " It does not support delta-neutral orders"))
            {
                return;
            }

            if (contract.ConId > 0 &&
                !CheckServerVersion(tickerId,
                                    MinServerVer.ContractConid,
                                    " It does not support ConId parameter"))
            {
                return;
            }

            if (!Util.StringIsEmpty(contract.TradingClass) &&
                !CheckServerVersion(tickerId,
                                    MinServerVer.TradingClass,
                                    " It does not support trading class parameter in reqMktData."))
            {
                return;
            }

            var version = 11;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketData);
            paramsList.AddParameter(version);
            paramsList.AddParameter(tickerId);

            if (ServerVersion >= MinServerVer.ContractConid)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);

            if (ServerVersion >= 15)
            {
                paramsList.AddParameter(contract.Multiplier);
            }

            paramsList.AddParameter(contract.Exchange);

            if (ServerVersion >= 14)
            {
                paramsList.AddParameter(contract.PrimaryExch);
            }

            paramsList.AddParameter(contract.Currency);

            if (ServerVersion >= 2)
            {
                paramsList.AddParameter(contract.LocalSymbol);
            }

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            if (ServerVersion >= 8 && Constants.BagSecType.Equals(contract.SecType))
            {
                if (contract.ComboLegs == null)
                {
                    paramsList.AddParameter(0);
                }
                else
                {
                    paramsList.AddParameter(contract.ComboLegs.Count);
                    for (var i = 0; i < contract.ComboLegs.Count; i++)
                    {
                        var leg = contract.ComboLegs[i];
                        paramsList.AddParameter(leg.ConId);
                        paramsList.AddParameter(leg.Ratio);
                        paramsList.AddParameter(leg.Action);
                        paramsList.AddParameter(leg.Exchange);
                    }
                }
            }

            if (ServerVersion >= MinServerVer.DeltaNeutral)
            {
                if (contract.DeltaNeutralContract != null)
                {
                    paramsList.AddParameter(true);
                    paramsList.AddParameter(contract.DeltaNeutralContract.ConId);
                    paramsList.AddParameter(contract.DeltaNeutralContract.Delta);
                    paramsList.AddParameter(contract.DeltaNeutralContract.Price);
                }
                else
                {
                    paramsList.AddParameter(false);
                }
            }

            if (ServerVersion >= 31)
            {
                paramsList.AddParameter(genericTickList);
            }

            if (ServerVersion >= MinServerVer.SnapshotMktData)
            {
                paramsList.AddParameter(snapshot);
            }

            if (ServerVersion >= MinServerVer.SmartComponents)
            {
                paramsList.AddParameter(regulatorySnaphsot);
            }

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(mktDataOptions);
            }

            CloseAndSend(tickerId, paramsList, lengthPos, EClientErrors.FailSendReqmkt);
        }

        /**
         * @brief Switches data type returned from reqMktData request to "frozen", "delayed" or "delayed-frozen" market data. Requires TWS/IBG v963+.\n
         * The API can receive frozen market data from Trader Workstation. Frozen market data is the last data recorded in our system.\n During normal trading hours, the API receives real-time market data. Invoking this function with argument 2 requests a switch to frozen data immediately or after the close.\n When the market reopens the next data the market data type will automatically switch back to real time if available.
         * @param marketDataType:
         *      by default only real-time (1) market data is enabled
         *      sending 1 (real-time) disables frozen, delayed and delayed-frozen market data
         *      sending 2 (frozen) enables frozen market data
         *      sending 3 (delayed) enables delayed and disables delayed-frozen market data
         *      sending 4 (delayed-frozen) enables delayed and delayed-frozen market data
         */
        public void ReqMarketDataType(int marketDataType)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqMarketDataType,
                                    " It does not support market data type requests."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketDataType);
            paramsList.AddParameter(version);
            paramsList.AddParameter(marketDataType);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqmarketdatatype);
        }

        /**
         * @brief Requests the contract's market depth (order book).\n This request must be direct-routed to an exchange and not smart-routed. The number of simultaneous market depth requests allowed in an account is calculated based on a formula that looks at an accounts equity, commissions, and quote booster packs.  
         * @param tickerId the request's identifier
         * @param contract the Contract for which the depth is being requested
         * @param numRows the number of rows on each side of the order book
         * @sa cancelMktDepth, EWrapper::updateMktDepth, EWrapper::updateMktDepthL2
         */
        public void ReqMarketDepth(int tickerId, Contract contract, int numRows, List<TagValue> mktDepthOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
            {
                if (!CheckServerVersion(tickerId,
                                        MinServerVer.TradingClass,
                                        " It does not support ConId nor TradingClass parameters in reqMktDepth."))
                {
                    return;
                }
            }

            const int version = 5;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketDepth);
            paramsList.AddParameter(version);
            paramsList.AddParameter(tickerId);

            // paramsList.AddParameter contract fields
            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);

            if (ServerVersion >= 15)
            {
                paramsList.AddParameter(contract.Multiplier);
            }

            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            if (ServerVersion >= 19)
            {
                paramsList.AddParameter(numRows);
            }

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(mktDepthOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqmktdepth);
        }

        /**
         * @brief Subscribes to IB's News Bulletins
         * @param allMessages if set to true, will return all the existing bulletins for the current day, set to false to receive only the new bulletins.
         * @sa cancelNewsBulletin, EWrapper::updateNewsBulletin
         */
        public void ReqNewsBulletins(bool allMessages)
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestNewsBulletins);
            paramsList.AddParameter(version);
            paramsList.AddParameter(allMessages);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailGeneric);
        }

        /**
         * @brief Requests all open orders places by this specific API client (identified by the API client id). For client ID 0, this will bind previous manual TWS orders.
         * @sa reqAllOpenOrders, reqAutoOpenOrders, placeOrder, cancelOrder, reqGlobalCancel, EWrapper::openOrder, EWrapper::orderStatus, EWrapper::openOrderEnd
         */
        public void ReqOpenOrders()
        {
            var version = 1;
            if (!CheckConnection())
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestOpenOrders);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendOorder);
        }

        /**
         * @brief Subscribes to position updates for all accessible accounts. All positions sent initially, and then only updates as positions change. 
         * @sa cancelPositions, EWrapper::position, EWrapper::positionEnd
         */
        public void ReqPositions()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.AcctSummary, " It does not support position requests."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestPositions);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqpositions);
        }

        /**
         * @brief Requests real time bars\n
         * Currently, only 5 seconds bars are provided. This request is subject to the same pacing as any historical data request: no more than 60 API queries in more than 600 seconds.\n Real time bars subscriptions are also included in the calculation of the number of Level 1 market data subscriptions allowed in an account. 
         * @param tickerId the request's unique identifier.
         * @param contract the Contract for which the depth is being requested
         * @param barSize currently being ignored
         * @param whatToShow the nature of the data being retrieved:
         *      - TRADES
         *      - MIDPOINT
         *      - BID
         *      - ASK
         * @param useRTH set to 0 to obtain the data which was also generated ourside of the Regular Trading Hours, set to 1 to obtain only the RTH data
         * @sa cancelRealTimeBars, EWrapper::realtimeBar
         */
        public void ReqRealTimeBars(int tickerId,
                                    Contract contract,
                                    int barSize,
                                    string whatToShow,
                                    bool useRth,
                                    List<TagValue> realTimeBarsOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(tickerId, MinServerVer.RealTimeBars, " It does not support real time bars."))
            {
                return;
            }

            if (!IsEmpty(contract.TradingClass) || contract.ConId > 0)
            {
                if (!CheckServerVersion(tickerId,
                                        MinServerVer.TradingClass,
                                        " It does not support ConId nor TradingClass parameters in reqRealTimeBars."))
                {
                    return;
                }
            }

            const int version = 3;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestRealTimeBars);
            paramsList.AddParameter(version);
            paramsList.AddParameter(tickerId);

            // paramsList.AddParameter contract fields
            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.ConId);
            }

            paramsList.AddParameter(contract.Symbol);
            paramsList.AddParameter(contract.SecType);
            paramsList.AddParameter(contract.LastTradeDateOrContractMonth);
            paramsList.AddParameter(contract.Strike);
            paramsList.AddParameter(contract.Right);
            paramsList.AddParameter(contract.Multiplier);
            paramsList.AddParameter(contract.Exchange);
            paramsList.AddParameter(contract.PrimaryExch);
            paramsList.AddParameter(contract.Currency);
            paramsList.AddParameter(contract.LocalSymbol);

            if (ServerVersion >= MinServerVer.TradingClass)
            {
                paramsList.AddParameter(contract.TradingClass);
            }

            paramsList.AddParameter(barSize); // this parameter is not currently used
            paramsList.AddParameter(whatToShow);
            paramsList.AddParameter(useRth);

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(realTimeBarsOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqrtbars);
        }

        /**
         * @brief Requests an XML list of scanner parameters valid in TWS. \n
		 * Not all parameters are valid from API scanner.
         * @sa reqScannerSubscription
         */
        public void ReqScannerParameters()
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestScannerParameters);
            paramsList.AddParameter(version);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqscannerparameters);
        }

        /**
         * @brief Starts a subscription to market scan results based on the provided parameters.
         * @param reqId the request's identifier
         * @param subscription summary of the scanner subscription including its filters.
         * @sa reqScannerParameters, ScannerSubscription, EWrapper::scannerData
         */
        public void ReqScannerSubscription(int reqId,
                                           ScannerSubscription subscription,
                                           List<TagValue> scannerSubscriptionOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 4;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestScannerSubscription);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);
            paramsList.AddParameterMax(subscription.NumberOfRows);
            paramsList.AddParameter(subscription.Instrument);
            paramsList.AddParameter(subscription.LocationCode);
            paramsList.AddParameter(subscription.ScanCode);
            paramsList.AddParameterMax(subscription.AbovePrice);
            paramsList.AddParameterMax(subscription.BelowPrice);
            paramsList.AddParameterMax(subscription.AboveVolume);
            paramsList.AddParameterMax(subscription.MarketCapAbove);
            paramsList.AddParameterMax(subscription.MarketCapBelow);
            paramsList.AddParameter(subscription.MoodyRatingAbove);
            paramsList.AddParameter(subscription.MoodyRatingBelow);
            paramsList.AddParameter(subscription.SpRatingAbove);
            paramsList.AddParameter(subscription.SpRatingBelow);
            paramsList.AddParameter(subscription.MaturityDateAbove);
            paramsList.AddParameter(subscription.MaturityDateBelow);
            paramsList.AddParameterMax(subscription.CouponRateAbove);
            paramsList.AddParameterMax(subscription.CouponRateBelow);
            paramsList.AddParameter(subscription.ExcludeConvertible);

            if (ServerVersion >= 25)
            {
                paramsList.AddParameterMax(subscription.AverageOptionVolumeAbove);
                paramsList.AddParameter(subscription.ScannerSettingPairs);
            }

            if (ServerVersion >= 27)
            {
                paramsList.AddParameter(subscription.StockTypeFilter);
            }

            if (ServerVersion >= MinServerVer.Linking)
            {
                paramsList.AddParameter(scannerSubscriptionOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqscanner);
        }

        /**
         * @brief Changes the TWS/GW log level.
		 * The default is 2 = ERROR\n
		 * 5 = DETAIL is required for capturing all API messages and troubleshooting API programs\n
         * Valid values are:\n
         * 1 = SYSTEM\n
         * 2 = ERROR\n
         * 3 = WARNING\n
         * 4 = INFORMATION\n
         * 5 = DETAIL\n
         */
        public void SetServerLogLevel(int logLevel)
        {
            if (!CheckConnection())
            {
                return;
            }

            const int version = 1;

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ChangeServerLog);
            paramsList.AddParameter(version);
            paramsList.AddParameter(logLevel);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendServerLogLevel);
        }

        /**
         * @brief For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
         */
        public void VerifyRequest(string apiName, string apiVersion)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support verification request."))
            {
                return;
            }

            if (!ExtraAuth)
            {
                ReportError(IncomingMessage.NotValid,
                            EClientErrors.FailSendVerifymessage,
                            " Intent to authenticate needs to be expressed during initial connect request.");
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.VerifyRequest);
            paramsList.AddParameter(version);
            paramsList.AddParameter(apiName);
            paramsList.AddParameter(apiVersion);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendVerifyrequest);
        }

        /**
         * @brief For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
         */
        public void VerifyMessage(string apiData)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support verification message sending."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.VerifyMessage);
            paramsList.AddParameter(version);
            paramsList.AddParameter(apiData);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendVerifymessage);
        }

        /**
         * @brief For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
         */
        public void VerifyAndAuthRequest(string apiName, string apiVersion, string opaqueIsvKey)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.LinkingAuth, " It does not support verification request."))
            {
                return;
            }

            if (!ExtraAuth)
            {
                ReportError(IncomingMessage.NotValid,
                            EClientErrors.FailSendVerifyandauthmessage,
                            " Intent to authenticate needs to be expressed during initial connect request.");
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);
            paramsList.AddParameter(OutgoingMessages.VerifyAndAuthRequest);
            paramsList.AddParameter(version);
            paramsList.AddParameter(apiName);
            paramsList.AddParameter(apiVersion);
            paramsList.AddParameter(opaqueIsvKey);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendVerifyandauthrequest);
        }

        /**
         * @brief For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
         */
        public void VerifyAndAuthMessage(string apiData, string xyzResponse)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.LinkingAuth, " It does not support verification message sending."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);
            paramsList.AddParameter(OutgoingMessages.VerifyAndAuthMessage);
            paramsList.AddParameter(version);
            paramsList.AddParameter(apiData);
            paramsList.AddParameter(xyzResponse);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendVerifyandauthmessage);
        }

        /**
         * @brief Requests all available Display Groups in TWS
         * @param requestId is the ID of this request
         */
        public void QueryDisplayGroups(int requestId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support queryDisplayGroups request."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.QueryDisplayGroups);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendQuerydisplaygroups);
        }

        /**
         * @brief Integrates API client and TWS window grouping. 
         *@param requestId is the Id chosen for this subscription request
         * @param groupId is the display group for integration
         */
        public void SubscribeToGroupEvents(int requestId, int groupId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support subscribeToGroupEvents request."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.SubscribeToGroupEvents);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(groupId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendSubscribetogroupevents);
        }

        /**
         * @brief Updates the contract displayed in a TWS Window Group
         * @param requestId is the ID chosen for this request
         * @param contractInfo is an encoded value designating a unique IB contract. Possible values include:
         * 1. none = empty selection
         * 2. contractID@exchange - any non-combination contract. Examples 8314@SMART for IBM SMART; 8314@ARCA for IBM ARCA
         * 3. combo= if any combo is selected
         * Note: This request from the API does not get a TWS response unless an error occurs. 
         */
        public void UpdateDisplayGroup(int requestId, string contractInfo)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support updateDisplayGroup request."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.UpdateDisplayGroup);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(contractInfo);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendUpdatedisplaygroup);
        }

        /**
         * @brief Cancels a TWS Window Group subscription
         */
        public void UnsubscribeFromGroupEvents(int requestId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Linking, " It does not support unsubscribeFromGroupEvents request."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.UnsubscribeFromGroupEvents);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendUnsubscribefromgroupevents);
        }

        /**
         * @brief Requests position subscription for account and/or model
		 * Initially all positions are returned, and then updates are returned for any position changes in real time.
         * @param requestId - Request's identifier
         * @param account - If an account Id is provided, only the account's positions belonging to the specified model will be delivered
         * @param modelCode - The code of the model's positions we are interested in.
         * @sa cancelPositionsMulti, EWrapper::positionMulti, EWrapper::positionMultiEnd
         */
        public void ReqPositionsMulti(int requestId, string account, string modelCode)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ModelsSupport, " It does not support positions multi requests."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestPositionsMulti);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(account);
            paramsList.AddParameter(modelCode);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqpositionsmulti);
        }

        /**
         * @brief Cancels positions request for account and/or model
         * @param requestId - the identifier of the request to be canceled.
         * @sa reqPositionsMulti
         */
        public void CancelPositionsMulti(int requestId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ModelsSupport,
                                    " It does not support positions multi cancellation."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPositionsMulti);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCanpositionsmulti);
        }

        /**
         * @brief Requests account updates for account and/or model
		 * @param reqId identifier to label the request 
		 * @param account account values can be requested for a particular account
		 * @param modelCode values can also be requested for a model
		 * @param ledgerAndNLV returns light-weight request; only currency positions as opposed to account values and currency positions
         * @sa cancelAccountUpdatesMulti, EWrapper::accountUpdateMulti, EWrapper::accountUpdateMultiEnd
         */
        public void ReqAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNlv)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ModelsSupport,
                                    " It does not support account updates multi requests."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestAccountUpdatesMulti);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(account);
            paramsList.AddParameter(modelCode);
            paramsList.AddParameter(ledgerAndNlv);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqaccountupdatesmulti);
        }

        /**
         * @brief Cancels account updates request for account and/or model
		 * @param requestId account subscription to cancel  
         * @sa reqAccountUpdatesMulti
         */
        public void CancelAccountUpdatesMulti(int requestId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ModelsSupport,
                                    " It does not support account updates multi cancellation."))
            {
                return;
            }

            const int version = 1;
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelAccountUpdatesMulti);
            paramsList.AddParameter(version);
            paramsList.AddParameter(requestId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCanaccountupdatesmulti);
        }

        /**
         * @brief Requests security definition option parameters for viewing a contract's option chain
         * @param reqId the ID chosen for the request
         * @param underlyingSymbol 
         * @param futFopExchange The exchange on which the returned options are trading. Can be set to the empty string "" for all exchanges.
         * @param underlyingSecType The type of the underlying security, i.e. STK
         * @param underlyingConId the contract ID of the underlying security
         * @sa EWrapper::securityDefinitionOptionParameter
         */
        public void ReqSecDefOptParams(int reqId,
                                       string underlyingSymbol,
                                       string futFopExchange,
                                       string underlyingSecType,
                                       int underlyingConId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.SecDefOptParamsReq,
                                    " It does not support security definition option parameters."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestSecurityDefinitionOptionalParameters);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(underlyingSymbol);
            paramsList.AddParameter(futFopExchange);
            paramsList.AddParameter(underlyingSecType);
            paramsList.AddParameter(underlyingConId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqsecdefoptparams);
        }

        /**
         * @brief Requests pre-defined Soft Dollar Tiers. This is only supported for registered professional advisors and hedge and mutual funds who have configured Soft Dollar Tiers in Account Management. Refer to: https://www.interactivebrokers.com/en/software/am/am/manageaccount/requestsoftdollars.htm?Highlight=soft%20dollar%20tier
         * @sa EWrapper::softDollarTiers
         */
        public void ReqSoftDollarTiers(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.SoftDollarTier,
                                    " It does not support soft dollar tier."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestSoftDollarTiers);
            paramsList.AddParameter(reqId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqsoftdollartiers);
        }

        /**
        * @brief Requests family codes for an account, for instance if it is a FA, IBroker, or associated account.
        * @sa EWrapper::familyCodes
        */
        public void ReqFamilyCodes()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqFamilyCodes,
                                    " It does not support family codes requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestFamilyCodes);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqfamilycodes);
        }

        /**
        * @brief Requests matching stock symbols
		* @param reqId id to specify the request
        * @param pattern - either start of ticker symbol or (for larger strings) company name
        * @sa EWrapper::symbolSamples
        */
        public void ReqMatchingSymbols(int reqId, string pattern)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqMatchingSymbols,
                                    " It does not support mathing symbols requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMatchingSymbols);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(pattern);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqmatchingsymbols);
        }

        /**
         * @brief Requests venues for which market data is returned to updateMktDepthL2 (those with market makers)
         * @sa EWrapper::mktDepthExchanges
         */
        public void ReqMktDepthExchanges()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqMktDepthExchanges,
                                    " It does not support market depth exchanges requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMktDepthExchanges);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqmktdepthexchanges);
        }

        /**
         * @brief Returns the mapping of single letter codes to exchange names given the mapping identifier
		 * @param reqId id of the request
		 * @param bboExchange mapping identifier received from EWrapper.tickReqParams
         * @sa EWrapper::smartComponents
             */
        public void ReqSmartComponents(int reqId, string bboExchange)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqMktDepthExchanges,
                                    " It does not support smart components request."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestSmartComponents);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(bboExchange);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqsmartcomponents);
        }

        /**
        * @brief Requests news providers which the user has subscribed to. 
        * @sa EWrapper::newsProviders
        */
        public void ReqNewsProviders()
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqNewsProviders,
                                    " It does not support news providers requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestNewsProviders);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqnewsproviders);
        }

        /**
         * @brief Requests news article body given articleId.
		 * @param requestId id of the request
		 * @param providerCode short code indicating news provider, e.g. FLY
		 * @param articleId id of the specific article
		 * @param newsArticleOptions reserved for internal use. Should be defined as null.
         * @sa EWrapper::newsArticle, 
         */
        public void ReqNewsArticle(int requestId,
                                   string providerCode,
                                   string articleId,
                                   List<TagValue> newsArticleOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqNewsArticle,
                                    " It does not support news article requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestNewsArticle);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(providerCode);
            paramsList.AddParameter(articleId);

            if (ServerVersion >= MinServerVer.NewsQueryOrigins)
            {
                paramsList.AddParameter(newsArticleOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqnewsarticle);
        }

        /**
        * @brief Requests historical news headlines
        * @param requestId
        * @param conId - contract id of ticker
        * @param providerCodes - a '+'-separated list of provider codes
        * @param startDateTime - marks the (exclusive) start of the date range. The format is yyyy-MM-dd HH:mm:ss.0
        * @param endDateTime - marks the (inclusive) end of the date range. The format is yyyy-MM-dd HH:mm:ss.0
        * @param totalResults - the maximum number of headlines to fetch (1 - 300)
		* @param historicalNewsOptions reserved for internal use. Should be defined as null.
        * @sa EWrapper::historicalNews, EWrapper::historicalNewsEnd
        */
        public void ReqHistoricalNews(int requestId,
                                      int conId,
                                      string providerCodes,
                                      string startDateTime,
                                      string endDateTime,
                                      int totalResults,
                                      List<TagValue> historicalNewsOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqHistoricalNews,
                                    " It does not support historical news requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestHistoricalNews);
            paramsList.AddParameter(requestId);
            paramsList.AddParameter(conId);
            paramsList.AddParameter(providerCodes);
            paramsList.AddParameter(startDateTime);
            paramsList.AddParameter(endDateTime);
            paramsList.AddParameter(totalResults);

            if (ServerVersion >= MinServerVer.NewsQueryOrigins)
            {
                paramsList.AddParameter(historicalNewsOptions);
            }

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqhistoricalnews);
        }

        /**
        * @brief Returns the timestamp of earliest available historical data for a contract and data type
        * @param tickerId - an identifier for the request
        * @param contract - contract object for which head timestamp is being requested
        * @param whatToShow - type of data for head timestamp - "BID", "ASK", "TRADES", etc
        * @param useRTH - use regular trading hours only, 1 for yes or 0 for no
        * @param formatDate - @param formatDate set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds
        * @sa headTimeStamp
        */

        public void ReqHeadTimestamp(int tickerId, Contract contract, string whatToShow, int useRth, int formatDate)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqHeadTimestamp,
                                    " It does not support head time stamp requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestHeadTimestamp);
            paramsList.AddParameter(tickerId);
            paramsList.AddParameter(contract);
            paramsList.AddParameter(useRth);
            paramsList.AddParameter(whatToShow);
            paramsList.AddParameter(formatDate);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqheadtimestamp);
        }

        /**
		* @brief Cancels a pending reqHeadTimeStamp request\n
		* @param tickerId Id of the request
		*/

        public void CancelHeadTimestamp(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.CancelHeadtimestamp,
                                    " It does not support head time stamp requests canceling."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelHeadTimestamp);
            paramsList.AddParameter(tickerId);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCancelheadtimestamp);
        }


        /**
        * @brief Returns data histogram of specified contract\n
        * @param tickerId - an identifier for the request\n
        * @param contract - Contract object for which histogram is being requested\n
        * @param useRTH - use regular trading hours only, 1 for yes or 0 for no\n
        * @param period - period of which data is being requested, e.g. "3 days"\n
        * @sa histogramData
        */

        public void ReqHistogramData(int tickerId, Contract contract, bool useRth, string period)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqHistogramData,
                                    " It does not support histogram data requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestHistogramData);
            paramsList.AddParameter(tickerId);
            paramsList.AddParameter(contract);
            paramsList.AddParameter(useRth);
            paramsList.AddParameter(period);
            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqhistogramdata);
        }

        /**
        * @brief Cancels an active data histogram request
        * @param tickerId - identifier specified in reqHistogramData request
        * @sa reqHistogramData, histogramData
        */

        public void CancelHistogramData(int tickerId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.ReqHistogramData,
                                    " It does not support histogram data requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelHistogramData);
            paramsList.AddParameter(tickerId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCancelhistogramdata);
        }

        /**
        * @brief Requests details about a given market rule\n
		* The market rule for an instrument on a particular exchange provides details about how the minimum price increment changes with price\n
		* A list of market rule ids can be obtained by invoking reqContractDetails on a particular contract. The returned market rule ID list will provide the market rule ID for the instrument in the correspond valid exchange list in contractDetails.\n
        * @param marketRuleId - the id of market rule\n
        * @sa EWrapper::marketRule
        */
        public void ReqMarketRule(int marketRuleId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.MarketRules,
                                    " It does not support market rule requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.RequestMarketRule);
            paramsList.AddParameter(marketRuleId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqmarketrule);
        }

        /**
        * @brief Creates subscription for real time daily PnL and unrealized PnL updates
        * @param account account for which to receive PnL updates
		* @param modelCode specify to request PnL updates for a specific model
        */

        public void ReqPnL(int reqId, string account, string modelCode)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Pnl,
                                    "  It does not support PnL requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqPnL);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(account);
            paramsList.AddParameter(modelCode);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqpnl);
        }

        /** 
		* @brief cancels subscription for real time updated daily PnL
		* params reqId
		*/

        public void CancelPnL(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Pnl,
                                    "  It does not support PnL requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPnL);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendCancelpnl);
        }

        /**
		* @brief Requests real time updates for daily PnL of individual positions
		* @param reqId
		* @param account account in which position exists
		* @param modelCode model in which position exists
		* @param conId contract ID (conId) of contract to receive daily PnL updates for.  
		* Note: does not return message if invalid conId is entered
		*/

        public void ReqPnLSingle(int reqId, string account, string modelCode, int conId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Pnl,
                                    "  It does not support PnL requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqPnLSingle);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(account);
            paramsList.AddParameter(modelCode);
            paramsList.AddParameter(conId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqpnlsingle);
        }

        /** 
		* @brief Cancels real time subscription for a positions daily PnL information
		* @param reqId
		*/

        public void CancelPnLSingle(int reqId)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.Pnl,
                                    "  It does not support PnL requests."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.CancelPnLSingle);
            paramsList.AddParameter(reqId);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqpnlsingle);
        }

        /**
		* @brief Requests historical Time&Sales data for an instrument
		* @param reqId id of the request
		* @param contract Contract object that is subject of query
		* @param startDateTime ,i.e. "20170701 12:01:00". Uses TWS timezone specified at login.
		* @param endDateTime ,i.e. "20170701 13:01:00". In TWS timezone. Exactly one of start time and end time has to be defined.
		* @param numberOfTicks Number of distinct data points. Max currently 1000 per request.
		* @param whatToShow (Bid_Ask, Midpoint, Trades) Type of data requested. 
		* @param useRth Data from regular trading hours (1), or all available hours (0)
		* @param ignoreSize A filter only used when the source price is Bid_Ask
		* @param miscOptions should be defined as <i>null</i>, reserved for internal use
		*/

        public void ReqHistoricalTicks(int reqId,
                                       Contract contract,
                                       string startDateTime,
                                       string endDateTime,
                                       int numberOfTicks,
                                       string whatToShow,
                                       int useRth,
                                       bool ignoreSize,
                                       List<TagValue> miscOptions)
        {
            if (!CheckConnection())
            {
                return;
            }

            if (!CheckServerVersion(MinServerVer.HistoricalTicks,
                                    "  It does not support historical ticks request."))
            {
                return;
            }

            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(OutgoingMessages.ReqHistoricalTicks);
            paramsList.AddParameter(reqId);
            paramsList.AddParameter(contract);
            paramsList.AddParameter(startDateTime);
            paramsList.AddParameter(endDateTime);
            paramsList.AddParameter(numberOfTicks);
            paramsList.AddParameter(whatToShow);
            paramsList.AddParameter(useRth);
            paramsList.AddParameter(ignoreSize);
            paramsList.AddParameter(miscOptions);

            CloseAndSend(paramsList, lengthPos, EClientErrors.FailSendReqhistoricalticks);
        }

        protected bool CheckServerVersion(int requiredVersion)
        {
            return CheckServerVersion(requiredVersion, "");
        }

        protected bool CheckServerVersion(int requestId, int requiredVersion)
        {
            return CheckServerVersion(requestId, requiredVersion, "");
        }

        protected bool CheckServerVersion(int requiredVersion, string updatetail)
        {
            return CheckServerVersion(IncomingMessage.NotValid, requiredVersion, updatetail);
        }

        protected bool CheckServerVersion(int tickerId, int requiredVersion, string updatetail)
        {
            if (ServerVersion < requiredVersion)
            {
                ReportUpdateTws(tickerId, updatetail);
                return false;
            }

            return true;
        }

        protected void CloseAndSend(BinaryWriter paramsList, uint lengthPos, CodeMsgPair error)
        {
            CloseAndSend(IncomingMessage.NotValid, paramsList, lengthPos, error);
        }

        protected void CloseAndSend(int reqId, BinaryWriter paramsList, uint lengthPos, CodeMsgPair error)
        {
            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(reqId, error.Code, error.Message);
                Close();
            }
        }

        protected abstract void CloseAndSend(BinaryWriter request, uint lengthPos);

        protected bool CheckConnection()
        {
            if (!IsConnected)
            {
                Wrapper.Error(IncomingMessage.NotValid,
                              EClientErrors.NotConnected.Code,
                              EClientErrors.NotConnected.Message);
                return false;
            }

            return true;
        }

        protected void ReportError(int reqId, CodeMsgPair error, string tail)
        {
            ReportError(reqId, error.Code, error.Message + tail);
        }

        protected void ReportUpdateTws(int reqId, string tail)
        {
            ReportError(reqId, EClientErrors.UpdateTws.Code, EClientErrors.UpdateTws.Message + tail);
        }

        protected void ReportUpdateTws(string tail)
        {
            ReportError(IncomingMessage.NotValid,
                        EClientErrors.UpdateTws.Code,
                        EClientErrors.UpdateTws.Message + tail);
        }

        protected void ReportError(int reqId, int code, string message)
        {
            Wrapper.Error(reqId, code, message);
        }

        protected void SendCancelRequest(OutgoingMessages msgType, int version, int reqId, CodeMsgPair errorMessage)
        {
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(msgType);
            paramsList.AddParameter(version);
            paramsList.AddParameter(reqId);
            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(reqId, errorMessage.Code, errorMessage.Message);
                Close();
            }
        }

        protected void SendCancelRequest(OutgoingMessages msgType, int version, CodeMsgPair errorMessage)
        {
            var paramsList = new BinaryWriter(new MemoryStream());
            var lengthPos = PrepareBuffer(paramsList);

            paramsList.AddParameter(msgType);
            paramsList.AddParameter(version);
            try
            {
                CloseAndSend(paramsList, lengthPos);
            }
            catch (Exception)
            {
                Wrapper.Error(IncomingMessage.NotValid, errorMessage.Code, errorMessage.Message);
                Close();
            }
        }

        protected bool VerifyOrderContract(Contract contract, int id)
        {
            if (ServerVersion < MinServerVer.SshortComboLegs)
            {
                if (contract.ComboLegs.Count > 0)
                {
                    ComboLeg comboLeg;
                    for (var i = 0; i < contract.ComboLegs.Count; ++i)
                    {
                        comboLeg = contract.ComboLegs[i];
                        if (comboLeg.ShortSaleSlot != 0 ||
                            !IsEmpty(comboLeg.DesignatedLocation))
                        {
                            ReportError(id,
                                        EClientErrors.UpdateTws,
                                        "  It does not support SSHORT flag for combo legs.");
                            return false;
                        }
                    }
                }
            }

            if (ServerVersion < MinServerVer.DeltaNeutral)
            {
                if (contract.DeltaNeutralContract != null)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support delta-neutral orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.PlaceOrderConid)
            {
                if (contract.ConId > 0)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support conId parameter.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.SecIdType)
            {
                if (!IsEmpty(contract.SecIdType) || !IsEmpty(contract.SecId))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support secIdType and secId parameters.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.Sshortx)
            {
                if (contract.ComboLegs.Count > 0)
                {
                    ComboLeg comboLeg;
                    for (var i = 0; i < contract.ComboLegs.Count; ++i)
                    {
                        comboLeg = contract.ComboLegs[i];
                        if (comboLeg.ExemptCode != -1)
                        {
                            ReportError(id,
                                        EClientErrors.UpdateTws,
                                        "  It does not support exemptCode parameter.");
                            return false;
                        }
                    }
                }
            }

            if (ServerVersion < MinServerVer.TradingClass)
            {
                if (!IsEmpty(contract.TradingClass))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support tradingClass parameters in placeOrder.");
                    return false;
                }
            }

            return true;
        }

        protected bool VerifyOrder(Order order, int id, bool isBagOrder)
        {
            if (ServerVersion < MinServerVer.ScaleOrders)
            {
                if (order.ScaleInitLevelSize != int.MaxValue ||
                    order.ScalePriceIncrement != double.MaxValue)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support Scale orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.WhatIfOrders)
            {
                if (order.WhatIf)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support what-if orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.ScaleOrders2)
            {
                if (order.ScaleSubsLevelSize != int.MaxValue)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support Subsequent Level Size for Scale orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.AlgoOrders)
            {
                if (!IsEmpty(order.AlgoStrategy))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support algo orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.NotHeld)
            {
                if (order.NotHeld)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support notHeld parameter.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.Sshortx)
            {
                if (order.ExemptCode != -1)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support exemptCode parameter.");
                    return false;
                }
            }


            if (ServerVersion < MinServerVer.HedgeOrders)
            {
                if (!IsEmpty(order.HedgeType))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support hedge orders.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.OptOutSmartRouting)
            {
                if (order.OptOutSmartRouting)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support optOutSmartRouting parameter.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.DeltaNeutralConid)
            {
                if (order.DeltaNeutralConId > 0 ||
                    !IsEmpty(order.DeltaNeutralSettlingFirm) ||
                    !IsEmpty(order.DeltaNeutralClearingAccount) ||
                    !IsEmpty(order.DeltaNeutralClearingIntent))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support deltaNeutral parameters: ConId, SettlingFirm, ClearingAccount, ClearingIntent");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.DeltaNeutralOpenClose)
            {
                if (!IsEmpty(order.DeltaNeutralOpenClose) ||
                    order.DeltaNeutralShortSale ||
                    order.DeltaNeutralShortSaleSlot > 0 ||
                    !IsEmpty(order.DeltaNeutralDesignatedLocation)
                )
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support deltaNeutral parameters: OpenClose, ShortSale, ShortSaleSlot, DesignatedLocation");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.ScaleOrders3)
            {
                if (order.ScalePriceIncrement > 0 && order.ScalePriceIncrement != double.MaxValue)
                {
                    if (order.ScalePriceAdjustValue != double.MaxValue ||
                        order.ScalePriceAdjustInterval != int.MaxValue ||
                        order.ScaleProfitOffset != double.MaxValue ||
                        order.ScaleAutoReset ||
                        order.ScaleInitPosition != int.MaxValue ||
                        order.ScaleInitFillQty != int.MaxValue ||
                        order.ScaleRandomPercent)
                    {
                        ReportError(id,
                                    EClientErrors.UpdateTws,
                                    "  It does not support Scale order parameters: PriceAdjustValue, PriceAdjustInterval, " +
                                    "ProfitOffset, AutoReset, InitPosition, InitFillQty and RandomPercent");
                        return false;
                    }
                }
            }

            if (ServerVersion < MinServerVer.OrderComboLegsPrice && isBagOrder)
            {
                if (order.OrderComboLegs.Count > 0)
                {
                    OrderComboLeg orderComboLeg;
                    for (var i = 0; i < order.OrderComboLegs.Count; ++i)
                    {
                        orderComboLeg = order.OrderComboLegs[i];
                        if (orderComboLeg.Price != double.MaxValue)
                        {
                            ReportError(id,
                                        EClientErrors.UpdateTws,
                                        "  It does not support per-leg prices for order combo legs.");
                            return false;
                        }
                    }
                }
            }

            if (ServerVersion < MinServerVer.TrailingPercent)
            {
                if (order.TrailingPercent != double.MaxValue)
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support trailing percent parameter.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.AlgoId && !IsEmpty(order.AlgoId))
            {
                ReportError(id, EClientErrors.UpdateTws, " It does not support algoId parameter");

                return false;
            }

            if (ServerVersion < MinServerVer.ScaleTable)
            {
                if (!IsEmpty(order.ScaleTable) || !IsEmpty(order.ActiveStartTime) || !IsEmpty(order.ActiveStopTime))
                {
                    ReportError(id,
                                EClientErrors.UpdateTws,
                                "  It does not support scaleTable, activeStartTime nor activeStopTime parameters.");
                    return false;
                }
            }

            if (ServerVersion < MinServerVer.ExtOperator && !IsEmpty(order.ExtOperator))
            {
                ReportError(id, EClientErrors.UpdateTws, " It does not support extOperator parameter");

                return false;
            }

            if (ServerVersion < MinServerVer.CashQty && order.CashQty != double.MaxValue)
            {
                ReportError(id, EClientErrors.UpdateTws, " It does not support cashQty parameter");

                return false;
            }

            if (ServerVersion < MinServerVer.DecisionMaker &&
                (!IsEmpty(order.Mifid2DecisionMaker) || !IsEmpty(order.Mifid2DecisionAlgo)))
            {
                ReportError(id, EClientErrors.UpdateTws, " It does not support MIFID II decision maker parameters");
            }

            if (ServerVersion < MinServerVer.DecisionMaker &&
                (!IsEmpty(order.Mifid2ExecutionTrader) || !IsEmpty(order.Mifid2ExecutionAlgo)))
            {
                ReportError(id, EClientErrors.UpdateTws, " It does not support MIFID II execution parameters");
            }

            if (ServerVersion < MinServerVer.AutoPriceForHedge && order.DontUseAutoPriceForHedge)
            {
                ReportError(id,
                            EClientErrors.UpdateTws,
                            " It does not support don't use auto price for hedge parameter");
            }

            return true;
        }

        private bool IsEmpty(string str)
        {
            return Util.StringIsEmpty(str);
        }

        private bool StringsAreEqual(string a, string b)
        {
            return string.Compare(a, b, true) == 0;
        }

        public int ReadInt()
        {
            return IPAddress.NetworkToHostOrder(new BinaryReader(TcpStream).ReadInt32());
        }

        public byte[] ReadAtLeastNBytes(int msgSize)
        {
            var buf = new byte[msgSize];

            return buf.Take(TcpStream.Read(buf, 0, msgSize)).ToArray();
        }

        public byte[] ReadByteArray(int msgSize)
        {
            return new BinaryReader(TcpStream).ReadBytes(msgSize);
        }
    }
}