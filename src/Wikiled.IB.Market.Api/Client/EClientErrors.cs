namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Contains all possible errors occurring on the client side. This errors are not sent by the TWS but rather generated as the result of malfunction within the
    * TWS API client.
    */
    public class EClientErrors
    {
        public static readonly CodeMsgPair AlreadyConnected = new CodeMsgPair(501, "Already Connected.");

        public static readonly CodeMsgPair ConnectFail = new CodeMsgPair(502,
                                                                          @"Couldn't connect to TWS. Confirm that ""Enable ActiveX and Socket Clients"" 
                            is enabled and connection port is the same as ""Socket Port"" on the TWS ""Edit->Global Configuration...->API->Settings"" menu. 
                            Live Trading ports: TWS: 7496; IB Gateway: 4001. Simulated Trading ports for new installations of version 954.1 or newer: 
                            TWS: 7497; IB Gateway: 4002");

        public static readonly CodeMsgPair UpdateTws =
            new CodeMsgPair(503, "The TWS is out of date and must be upgraded.");

        public static readonly CodeMsgPair NotConnected = new CodeMsgPair(504, "Not connected");
        public static readonly CodeMsgPair UnknownId = new CodeMsgPair(505, "Fatal Error: Unknown message id.");

        public static readonly CodeMsgPair FailSendReqmkt =
            new CodeMsgPair(510, "Request Market Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanmkt =
            new CodeMsgPair(511, "Cancel Market Data Sending Error - ");

        public static readonly CodeMsgPair FailSendOrder = new CodeMsgPair(512, "Order Sending Error - ");

        public static readonly CodeMsgPair FailSendAcct =
            new CodeMsgPair(513, "Account Update Request Sending Error -");

        public static readonly CodeMsgPair FailSendExec =
            new CodeMsgPair(514, "Request For Executions Sending Error -");

        public static readonly CodeMsgPair FailSendCorder = new CodeMsgPair(515, "Cancel Order Sending Error -");

        public static readonly CodeMsgPair
            FailSendOorder = new CodeMsgPair(516, "Request Open Order Sending Error -");

        public static readonly CodeMsgPair UnknownContract =
            new CodeMsgPair(517, "Unknown contract. Verify the contract details supplied.");

        public static readonly CodeMsgPair FailSendReqcontract =
            new CodeMsgPair(518, "Request Contract Data Sending Error - ");

        public static readonly CodeMsgPair FailSendReqmktdepth =
            new CodeMsgPair(519, "Request Market Depth Sending Error - ");

        public static readonly CodeMsgPair FailSendCanmktdepth =
            new CodeMsgPair(520, "Cancel Market Depth Sending Error - ");

        public static readonly CodeMsgPair FailSendServerLogLevel =
            new CodeMsgPair(521, "Set Server Log Level Sending Error - ");

        public static readonly CodeMsgPair FailSendFaRequest =
            new CodeMsgPair(522, "FA Information Request Sending Error - ");

        public static readonly CodeMsgPair FailSendFaReplace =
            new CodeMsgPair(523, "FA Information Replace Sending Error - ");

        public static readonly CodeMsgPair FailSendReqscanner =
            new CodeMsgPair(524, "Request Scanner Subscription Sending Error - ");

        public static readonly CodeMsgPair FailSendCanscanner =
            new CodeMsgPair(525, "Cancel Scanner Subscription Sending Error - ");

        public static readonly CodeMsgPair FailSendReqscannerparameters =
            new CodeMsgPair(526, "Request Scanner Parameter Sending Error - ");

        public static readonly CodeMsgPair FailSendReqhistdata =
            new CodeMsgPair(527, "Request Historical Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanhistdata =
            new CodeMsgPair(528, "Request Historical Data Sending Error - ");

        public static readonly CodeMsgPair FailSendReqrtbars =
            new CodeMsgPair(529, "Request Real-time Bar Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanrtbars =
            new CodeMsgPair(530, "Cancel Real-time Bar Data Sending Error - ");

        public static readonly CodeMsgPair FailSendReqcurrtime =
            new CodeMsgPair(531, "Request Current Time Sending Error - ");

        public static readonly CodeMsgPair FailSendReqfunddata =
            new CodeMsgPair(532, "Request Fundamental Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanfunddata =
            new CodeMsgPair(533, "Cancel Fundamental Data Sending Error - ");

        public static readonly CodeMsgPair FailSendReqcalcimpliedvolat =
            new CodeMsgPair(534, "Request Calculate Implied Volatility Sending Error - ");

        public static readonly CodeMsgPair FailSendReqcalcoptionprice =
            new CodeMsgPair(535, "Request Calculate Option Price Sending Error - ");

        public static readonly CodeMsgPair FailSendCancalcimpliedvolat =
            new CodeMsgPair(536, "Cancel Calculate Implied Volatility Sending Error - ");

        public static readonly CodeMsgPair FailSendCancalcoptionprice =
            new CodeMsgPair(537, "Cancel Calculate Option Price Sending Error - ");

        public static readonly CodeMsgPair FailSendReqglobalcancel =
            new CodeMsgPair(538, "Request Global Cancel Sending Error - ");

        public static readonly CodeMsgPair FailSendReqmarketdatatype =
            new CodeMsgPair(539, "Request Market Data Type Sending Error - ");

        public static readonly CodeMsgPair FailSendReqpositions =
            new CodeMsgPair(540, "Request Positions Sending Error - ");

        public static readonly CodeMsgPair FailSendCanpositions =
            new CodeMsgPair(541, "Cancel Positions Sending Error - ");

        public static readonly CodeMsgPair FailSendReqaccountdata =
            new CodeMsgPair(542, "Request Account Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanaccountdata =
            new CodeMsgPair(543, "Cancel Account Data Sending Error - ");

        public static readonly CodeMsgPair FailSendVerifyrequest =
            new CodeMsgPair(544, "Verify Request Sending Error - ");

        public static readonly CodeMsgPair FailSendVerifymessage =
            new CodeMsgPair(545, "Verify Message Sending Error - ");

        public static readonly CodeMsgPair FailSendQuerydisplaygroups =
            new CodeMsgPair(546, "Query Display Groups Sending Error - ");

        public static readonly CodeMsgPair FailSendSubscribetogroupevents =
            new CodeMsgPair(547, "Subscribe To Group Events Sending Error - ");

        public static readonly CodeMsgPair FailSendUpdatedisplaygroup =
            new CodeMsgPair(548, "Update Display Group Sending Error - ");

        public static readonly CodeMsgPair FailSendUnsubscribefromgroupevents =
            new CodeMsgPair(549, "Unsubscribe From Group Events Sending Error - ");

        public static readonly CodeMsgPair BadLength = new CodeMsgPair(507, "Bad message length");
        public static readonly CodeMsgPair BadMessage = new CodeMsgPair(508, "Bad message");
        public static readonly CodeMsgPair UnsupportedVersion = new CodeMsgPair(506, "Unsupported version");

        public static readonly CodeMsgPair FailSendVerifyandauthrequest =
            new CodeMsgPair(551, "Verify And Auth Request Sending Error - ");

        public static readonly CodeMsgPair FailSendVerifyandauthmessage =
            new CodeMsgPair(552, "Verify And Auth Message Sending Error - ");

        public static readonly CodeMsgPair FailSendReqpositionsmulti =
            new CodeMsgPair(553, "Request Positions Multi Sending Error - ");

        public static readonly CodeMsgPair FailSendCanpositionsmulti =
            new CodeMsgPair(554, "Cancel Positions Multi Sending Error - ");

        public static readonly CodeMsgPair FailSendReqaccountupdatesmulti =
            new CodeMsgPair(555, "Request Account Updates Multi Sending Error - ");

        public static readonly CodeMsgPair FailSendCanaccountupdatesmulti =
            new CodeMsgPair(556, "Cancel Account Updates Multi Sending Error - ");

        public static readonly CodeMsgPair FailSendReqsecdefoptparams =
            new CodeMsgPair(557, "Request Security Definition Option Parameters Sending Error - ");

        public static readonly CodeMsgPair FailSendReqsoftdollartiers =
            new CodeMsgPair(558, "Request Soft Dollar Tiers Sending Error - ");

        public static readonly CodeMsgPair FailSendReqfamilycodes =
            new CodeMsgPair(559, "Request Family Codes Sending Error - ");

        public static readonly CodeMsgPair FailSendReqmatchingsymbols =
            new CodeMsgPair(560, "Request Matching Symbols Sending Error - ");

        public static readonly CodeMsgPair FailSendReqmktdepthexchanges =
            new CodeMsgPair(561, "Request Market Depth Exchanges Sending Error - ");

        public static readonly CodeMsgPair FailSendReqsmartcomponents =
            new CodeMsgPair(562, "Request Smart Components Sending Error - ");

        public static readonly CodeMsgPair FailSendReqnewsproviders =
            new CodeMsgPair(563, "Request News Providers Sending Error - ");

        public static readonly CodeMsgPair FailSendReqnewsarticle =
            new CodeMsgPair(564, "Request News Article Sending Error - ");

        public static readonly CodeMsgPair FailSendReqhistoricalnews =
            new CodeMsgPair(565, "Request Historical News Sending Error - ");

        public static readonly CodeMsgPair FailSendReqheadtimestamp =
            new CodeMsgPair(566, "Request Head Time Stamp Sending Error - ");

        public static readonly CodeMsgPair FailSendReqhistogramdata =
            new CodeMsgPair(567, "Request Histogram Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCancelhistogramdata =
            new CodeMsgPair(568, "Cancel Request Histogram Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCancelheadtimestamp =
            new CodeMsgPair(569, "Cancel Head Time Stamp Sending Error - ");

        public static readonly CodeMsgPair FailSendReqmarketrule =
            new CodeMsgPair(570, "Request Market Rule Sending Error - ");

        public static readonly CodeMsgPair FailSendReqpnl = new CodeMsgPair(571, "Request PnL Sending Error - ");
        public static readonly CodeMsgPair FailSendCancelpnl = new CodeMsgPair(572, "Cancel PnL Sending Error - ");
        public static readonly CodeMsgPair FailSendReqpnlsingle = new CodeMsgPair(573, "Request PnL Single Error - ");

        public static readonly CodeMsgPair FailSendCancelpnlsingle =
            new CodeMsgPair(574, "Cancel PnL Single Sending Error - ");

        public static readonly CodeMsgPair FailSendReqhistoricalticks =
            new CodeMsgPair(575, "Request Historical Ticks Error - ");

        public static readonly CodeMsgPair FailSendReqtickbytickdata =
            new CodeMsgPair(576, "Request Tick-By-Tick Data Sending Error - ");

        public static readonly CodeMsgPair FailSendCanceltickbytickdata =
            new CodeMsgPair(577, "Cancel Tick-By-Tick Data Sending Error - ");

        public static readonly CodeMsgPair FailGeneric =
            new CodeMsgPair(-1, "Specific error message needs to be given for these requests! ");
    }

/**
  * @brief associates error code and error message as a pair. 
  */
}