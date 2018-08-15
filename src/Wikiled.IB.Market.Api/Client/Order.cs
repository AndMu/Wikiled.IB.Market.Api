using System.Collections.Generic;

namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class Order
     * @brief The order's description.
     * @sa Contract, OrderComboLeg, OrderState
     */
    public class Order
    {
        public static int Customer = 0;
        public static int Firm = 1;
        public static char OptUnknown = '?';
        public static char OptBrokerDealer = 'b';
        public static char OptCustomer = 'c';
        public static char OptFirm = 'f';
        public static char OptIsemm = 'm';
        public static char OptFarmm = 'n';
        public static char OptSpecialist = 'y';
        public static int AuctionMatch = 1;
        public static int AuctionImprovement = 2;
        public static int AuctionTransparent = 3;

        public static string EmptyStr = "";

        // Clearing info

        //GTC orders

        //algoId

        // ALGO ORDERS ONLY

        // BOX or VOL ORDERS ONLY
        // 1=AUCTION_MATCH, 2=AUCTION_IMPROVEMENT, 3=AUCTION_TRANSPARENT

        // COMBO ORDERS ONLY
        // EFP orders only

        // EFP orders only

        // native cash quantity

        // True beneficiary of the order

        // "" (Default), "IB", "Away", "PTA" (PostTrade)

        // set when slot=2 only.

        // SMART routing only

        // don't use auto price for hedge

        // Financial advisors only 

        // FORMAT: 20060505 08:00:00 {time zone}

        // FORMAT: 20060505 08:00:00 {time zone}

        // beta value for beta hedge, ratio for pair hedge

        // HEDGE ORDERS ONLY
        // 'D' - delta, 'B' - beta, 'F' - FX, 'P' - pair

        // Not Held

        // one cancels all group name

        // 1 = CANCEL_WITH_BLOCK, 2 = REDUCE_WITH_BLOCK, 3 = REDUCE_NON_BLOCK

        // Institutional orders only
        // O=Open, C=Close

        // order combo legs

        // main order fields

        // 0=Customer, 1=Firm

        // Parent order Id, to associate Auto STP or TRAIL orders with the original order.

        // REL orders only

        // 1=Average, 2 = BidOrAsk

        // Individual = 'I', Agency = 'A', AgentOtherMember = 'W', IndividualPTIA = 'J', AgencyPTIA = 'U', AgentOtherMemberPTIA = 'M', IndividualPT = 'K', AgencyPT = 'Y', AgentOtherMemberPT = 'N'

        // SCALE ORDERS ONLY

        // 1 if you hold the shares, 2 if they will be delivered from elsewhere.  Only for Action="SSHORT

        // Smart combo routing params

        // BOX ORDERS ONLY

        // pegged to stock or VOL orders

        // extended order fields
        // "Time in Force" - DAY, GTC, etc.

        // for TRAILLIMIT orders only

        // if false, order will be created but not transmitted

        // 0=Default, 1=Double_Bid_Ask, 2=Last, 3=Double_Last, 4=Bid_Ask, 7=Last_or_Bid_Ask, 8=Mid-point

        // VOLATILITY ORDERS ONLY

        // 1=daily, 2=annual

        // What-if

        public Order()
        {
            LmtPrice = double.MaxValue;
            AuxPrice = double.MaxValue;
            ActiveStartTime = EmptyStr;
            ActiveStopTime = EmptyStr;
            OutsideRth = false;
            OpenClose = "O";
            Origin = Customer;
            Transmit = true;
            DesignatedLocation = EmptyStr;
            ExemptCode = -1;
            MinQty = int.MaxValue;
            PercentOffset = double.MaxValue;
            NbboPriceCap = double.MaxValue;
            OptOutSmartRouting = false;
            StartingPrice = double.MaxValue;
            StockRefPrice = double.MaxValue;
            Delta = double.MaxValue;
            StockRangeLower = double.MaxValue;
            StockRangeUpper = double.MaxValue;
            Volatility = double.MaxValue;
            VolatilityType = int.MaxValue;
            DeltaNeutralOrderType = EmptyStr;
            DeltaNeutralAuxPrice = double.MaxValue;
            DeltaNeutralConId = 0;
            DeltaNeutralSettlingFirm = EmptyStr;
            DeltaNeutralClearingAccount = EmptyStr;
            DeltaNeutralClearingIntent = EmptyStr;
            DeltaNeutralOpenClose = EmptyStr;
            DeltaNeutralShortSale = false;
            DeltaNeutralShortSaleSlot = 0;
            DeltaNeutralDesignatedLocation = EmptyStr;
            ReferencePriceType = int.MaxValue;
            TrailStopPrice = double.MaxValue;
            TrailingPercent = double.MaxValue;
            BasisPoints = double.MaxValue;
            BasisPointsType = int.MaxValue;
            ScaleInitLevelSize = int.MaxValue;
            ScaleSubsLevelSize = int.MaxValue;
            ScalePriceIncrement = double.MaxValue;
            ScalePriceAdjustValue = double.MaxValue;
            ScalePriceAdjustInterval = int.MaxValue;
            ScaleProfitOffset = double.MaxValue;
            ScaleAutoReset = false;
            ScaleInitPosition = int.MaxValue;
            ScaleInitFillQty = int.MaxValue;
            ScaleRandomPercent = false;
            ScaleTable = EmptyStr;
            WhatIf = false;
            NotHeld = false;
            Conditions = new List<OrderCondition>();
            TriggerPrice = double.MaxValue;
            LmtPriceOffset = double.MaxValue;
            AdjustedStopPrice = double.MaxValue;
            AdjustedStopLimitPrice = double.MaxValue;
            AdjustedTrailingAmount = double.MaxValue;
            ExtOperator = EmptyStr;
            Tier = new SoftDollarTier(EmptyStr, EmptyStr, EmptyStr);
            CashQty = double.MaxValue;
            Mifid2DecisionMaker = EmptyStr;
            Mifid2DecisionAlgo = EmptyStr;
            Mifid2ExecutionTrader = EmptyStr;
            Mifid2ExecutionAlgo = EmptyStr;
            DontUseAutoPriceForHedge = false;
        }

        /**
         * @brief The API client's order id.
         */
        public int OrderId { get; set; }

        public bool Solicited { get; set; }

        /**
         * @brief The API client id which placed the order.
         */
        public int ClientId { get; set; }

        /**
         * @brief The Host order identifier.
         */
        public int PermId { get; set; }

        /**
         * @brief Identifies the side.
         * Generally available values are BUY, SELL. 
		 * Additionally, SSHORT, SLONG are available in some institutional-accounts only.
		 * For general account types, a SELL order will be able to enter a short position automatically if the order quantity is larger than your current long position.
         * SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
		 * SLONG is available in specially-configured institutional accounts to indicate that long position not yet delivered is being sold.	
         */
        public string Action { get; set; }

        /**
         * @brief The number of positions being bought/sold.
         */
        public double TotalQuantity { get; set; }

        /**
         * @brief The order's type.
         * Available Orders are at https://www.interactivebrokers.com/en/software/api/apiguide/tables/supported_order_types.htm 
         */
        public string OrderType { get; set; }

        /**
         * @brief The LIMIT price.
         * Used for limit, stop-limit and relative orders. In all other cases specify zero. For relative orders with no limit price, also specify zero.
         */
        public double LmtPrice { get; set; }

        /**
         * @brief Generic field to contain the stop price for STP LMT orders, trailing amount, etc.
         */
        public double AuxPrice { get; set; }

        /**
          * @brief The time in force.
         * Valid values are: \n
         *      DAY - Valid for the day only.\n
         *      GTC - Good until canceled. The order will continue to work within the system and in the marketplace until it executes or is canceled. GTC orders will be automatically be cancelled under the following conditions:
         *          \t\t If a corporate action on a security results in a stock split (forward or reverse), exchange for shares, or distribution of shares.
         *          \t\t If you do not log into your IB account for 90 days.\n
         *          \t\t At the end of the calendar quarter following the current quarter. For example, an order placed during the third quarter of 2011 will be canceled at the end of the first quarter of 2012. If the last day is a non-trading day, the cancellation will occur at the close of the final trading day of that quarter. For example, if the last day of the quarter is Sunday, the orders will be cancelled on the preceding Friday.\n
         *          \t\t Orders that are modified will be assigned a new “Auto Expire” date consistent with the end of the calendar quarter following the current quarter.\n
         *          \t\t Orders submitted to IB that remain in force for more than one day will not be reduced for dividends. To allow adjustment to your order price on ex-dividend date, consider using a Good-Til-Date/Time (GTD) or Good-after-Time/Date (GAT) order type, or a combination of the two.\n
         *      IOC - Immediate or Cancel. Any portion that is not filled as soon as it becomes available in the market is canceled.\n
         *      GTD. - Good until Date. It will remain working within the system and in the marketplace until it executes or until the close of the market on the date specified\n
         *      OPG - Use OPG to send a market-on-open (MOO) or limit-on-open (LOO) order.\n
         *      FOK - If the entire Fill-or-Kill order does not execute as soon as it becomes available, the entire order is canceled.\n
         *      DTC - Day until Canceled \n
          */
        public string Tif { get; set; }


        /**
         * @brief One-Cancels-All group identifier.
         */
        public string OcaGroup { get; set; }

        /**
         * @brief Tells how to handle remaining orders in an OCA group when one order or part of an order executes.
         * Valid values are:\n
         *      \t\t 1 = Cancel all remaining orders with block.\n
         *      \t\t 2 = Remaining orders are proportionately reduced in size with block.\n
         *      \t\t 3 = Remaining orders are proportionately reduced in size with no block.\n
         * If you use a value "with block" gives your order has overfill protection. This means that only one order in the group will be routed at a time to remove the possibility of an overfill.
         */
        public int OcaType { get; set; }

        /**
         * @brief The order reference.
         * Intended for institutional customers only, although all customers may use it to identify the API client that sent the order when multiple API clients are running.
         */
        public string OrderRef { get; set; }

        /**
         * @brief Specifies whether the order will be transmitted by TWS. If set to false, the order will be created at TWS but will not be sent.
         */
        public bool Transmit { get; set; }

        /**
         * @brief The order ID of the parent order, used for bracket and auto trailing stop orders.
         */
        public int ParentId { get; set; }

        /**
         * @brief If set to true, specifies that the order is an ISE Block order.
         */
        public bool BlockOrder { get; set; }

        /**
         * @brief If set to true, specifies that the order is a Sweep-to-Fill order.
         */
        public bool SweepToFill { get; set; }

        /**
         * @brief The publicly disclosed order size, used when placing Iceberg orders.
         */
        public int DisplaySize { get; set; }

        /**
         * @brief Specifies how Simulated Stop, Stop-Limit and Trailing Stop orders are triggered.
         * Valid values are:\n
         *  0 - The default value. The "double bid/ask" function will be used for orders for OTC stocks and US options. All other orders will used the "last" function.\n
         *  1 - use "double bid/ask" function, where stop orders are triggered based on two consecutive bid or ask prices.\n
         *  2 - "last" function, where stop orders are triggered based on the last price.\n
         *  3 double last function.\n
         *  4 bid/ask function.\n
         *  7 last or bid/ask function.\n
         *  8 mid-point function.\n
         */
        public int TriggerMethod { get; set; }

        /**
         * @brief If set to true, allows orders to also trigger or fill outside of regular trading hours.
         */
        public bool OutsideRth { get; set; }

        /**
         * @brief If set to true, the order will not be visible when viewing the market depth. 
         * This option only applies to orders routed to the ISLAND exchange.
         */
        public bool Hidden { get; set; }

        /**
         * @brief Specifies the date and time after which the order will be active.
         * Format: yyyymmdd hh:mm:ss {optional Timezone}
         */
        public string GoodAfterTime { get; set; }

        /**
         * @brief The date and time until the order will be active.
         * You must enter GTD as the time in force to use this string. The trade's "Good Till Date," format "YYYYMMDD hh:mm:ss (optional time zone)"
         */
        public string GoodTillDate { get; set; }

        /**
         * @brief Overrides TWS constraints.
         * Precautionary constraints are defined on the TWS Presets page, and help ensure tha tyour price and size order values are reasonable. Orders sent from the API are also validated against these safety constraints, and may be rejected if any constraint is violated. To override validation, set this parameter’s value to True.
         * 
         */
        public bool OverridePercentageConstraints { get; set; }

        /**
         * @brief -
         * Individual = 'I'\n
         * Agency = 'A'\n
         * AgentOtherMember = 'W'\n
         * IndividualPTIA = 'J'\n
         * AgencyPTIA = 'U'\n
         * AgentOtherMemberPTIA = 'M'\n
         * IndividualPT = 'K'\n
         * AgencyPT = 'Y'\n
         * AgentOtherMemberPT = 'N'\n
         */
        public string Rule80A { get; set; }

        /**
         * @brief Indicates whether or not all the order has to be filled on a single execution.
         */
        public bool AllOrNone { get; set; }

        /**
         * @brief Identifies a minimum quantity order type.
         */
        public int MinQty { get; set; }

        /**
         * @brief The percent offset amount for relative orders.
         */
        public double PercentOffset { get; set; }

        /**
         * @brief Trail stop price for TRAILIMIT orders.
         */
        public double TrailStopPrice { get; set; }

        /**
         * @brief Specifies the trailing amount of a trailing stop order as a percentage.
         * Observe the following guidelines when using the trailingPercent field:\n
         *    - This field is mutually exclusive with the existing trailing amount. That is, the API client can send one or the other but not both.\n
         *    - This field is read AFTER the stop price (barrier price) as follows: deltaNeutralAuxPrice stopPrice, trailingPercent, scale order attributes\n
         *    - The field will also be sent to the API in the openOrder message if the API client version is >= 56. It is sent after the stopPrice field as follows: stopPrice, trailingPct, basisPoint\n
         */
        public double TrailingPercent { get; set; }

        /**
         * @brief The Financial Advisor group the trade will be allocated to.
         * Use an empty string if not applicable.
         */
        public string FaGroup { get; set; }

        /**
         * @brief The Financial Advisor allocation profile the trade will be allocated to.
         * Use an empty string if not applicable.
         */
        public string FaProfile { get; set; }

        /**
         * @brief The Financial Advisor allocation method the trade will be allocated to.
         * Use an empty string if not applicable.
         */
        public string FaMethod { get; set; }

        /**
         * @brief The Financial Advisor percentage concerning the trade's allocation.
         * Use an empty string if not applicable.
         */
        public string FaPercentage { get; set; }


        /**
         * @brief For institutional customers only. Valid values are O (open), C (close).
         * Available for institutional clients to determine if this order is to open or close a position. 
		 * When Action = "BUY" and OpenClose = "O" this will open a new position. 
		 * When Action = "BUY" and OpenClose = "C" this will close and existing short position.
         */
        public string OpenClose { get; set; }


        /**
         * @brief The order's origin. 
         * Same as TWS "Origin" column. Identifies the type of customer from which the order originated. Valid values are 0 (customer), 1 (firm).
         */
        public int Origin { get; set; }

        /**
         * @brief -
         * For institutions only. Valid values are: 1 (broker holds shares) or 2 (shares come from elsewhere).
         */
        public int ShortSaleSlot { get; set; }

        /**
         * @brief Used only when shortSaleSlot is 2.
         * For institutions only. Indicates the location where the shares to short come from. Used only when short 
         * sale slot is set to 2 (which means that the shares to short are held elsewhere and not with IB).
         */
        public string DesignatedLocation { get; set; }

        /**
         * @brief Only available with IB Execution-Only accounts with applicable securities
	 * Mark order as exempt from short sale uptick rule 
         */
        public int ExemptCode { get; set; }

        /**
          * @brief The amount off the limit price allowed for discretionary orders.
          */
        public double DiscretionaryAmt { get; set; }

        /**
         * @brief Trade with electronic quotes.
         */
        public bool ETradeOnly { get; set; }

        /**
         * @brief Trade with firm quotes.
         */
        public bool FirmQuoteOnly { get; set; }

        /**
         * @brief Maximum smart order distance from the NBBO.
         */
        public double NbboPriceCap { get; set; }

        /**
         * @brief Use to opt out of default SmartRouting for orders routed directly to ASX.
         * This attribute defaults to false unless explicitly set to true. When set to false, orders routed directly to ASX will NOT use SmartRouting. When set to true, orders routed directly to ASX orders WILL use SmartRouting.
         */
        public bool OptOutSmartRouting { get; set; }

        /**
         * @brief - 
         * For BOX orders only. Values include:
         *      1 - match \n
         *      2 - improvement \n
         *      3 - transparent \n
         */
        public int AuctionStrategy { get; set; }

        /**
         * @brief The auction's starting price.
         * For BOX orders only.
         */
        public double StartingPrice { get; set; }

        /**
         * @brief The stock's reference price.
         * The reference price is used for VOL orders to compute the limit price sent to an exchange (whether or not Continuous Update is selected), and for price range monitoring.
         */
        public double StockRefPrice { get; set; }

        /**
         * @brief The stock's Delta.
         * For orders on BOX only.
         */
        public double Delta { get; set; }

        /**
          * @brief The lower value for the acceptable underlying stock price range.
          * For price improvement option orders on BOX and VOL orders with dynamic management.
          */
        public double StockRangeLower { get; set; }

        /**
         * @brief The upper value for the acceptable underlying stock price range.
         * For price improvement option orders on BOX and VOL orders with dynamic management.
         */
        public double StockRangeUpper { get; set; }


        /**
         * @brief The option price in volatility, as calculated by TWS' Option Analytics.
         * This value is expressed as a percent and is used to calculate the limit price sent to the exchange.
         */
        public double Volatility { get; set; }

        /**
         * @brief
         * Values include:\n
         *      1 - Daily Volatility
         *      2 - Annual Volatility
         */
        public int VolatilityType { get; set; }

        /**
         * @brief Specifies whether TWS will automatically update the limit price of the order as the underlying price moves.
         * VOL orders only.
         */
        public int ContinuousUpdate { get; set; }

        /**
         * @brief Specifies how you want TWS to calculate the limit price for options, and for stock range price monitoring.
         * VOL orders only. Valid values include: \n
         *      1 - Average of NBBO \n
         *      2 - NBB or the NBO depending on the action and right. \n
         */
        public int ReferencePriceType { get; set; }

        /**
         * @brief Enter an order type to instruct TWS to submit a delta neutral trade on full or partial execution of the VOL order.
         * VOL orders only. For no hedge delta order to be sent, specify NONE.
         */
        public string DeltaNeutralOrderType { get; set; }

        /**
         * @brief Use this field to enter a value if the value in the deltaNeutralOrderType field is an order type that requires an Aux price, such as a REL order. 
         * VOL orders only.
         */
        public double DeltaNeutralAuxPrice { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public int DeltaNeutralConId { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public string DeltaNeutralSettlingFirm { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public string DeltaNeutralClearingAccount { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public string DeltaNeutralClearingIntent { get; set; }

        /**
         * @brief Specifies whether the order is an Open or a Close order and is used when the hedge involves a CFD and and the order is clearing away.
         */
        public string DeltaNeutralOpenClose { get; set; }

        /**
         * @brief Used when the hedge involves a stock and indicates whether or not it is sold short.
         */
        public bool DeltaNeutralShortSale { get; set; }

        /**
         * @brief -
         * Has a value of 1 (the clearing broker holds shares) or 2 (delivered from a third party). If you use 2, then you must specify a deltaNeutralDesignatedLocation.
         */
        public int DeltaNeutralShortSaleSlot { get; set; }

        /**
         * @brief -
         * Used only when deltaNeutralShortSaleSlot = 2.
         */
        public string DeltaNeutralDesignatedLocation { get; set; }

        /**
         * @brief - DOC_TODO
         * For EFP orders only.
         */
        public double BasisPoints { get; set; }

        /**
         * @brief - DOC_TODO
         * For EFP orders only.
         */
        public int BasisPointsType { get; set; }

        /**
         * @brief Defines the size of the first, or initial, order component.
         * For Scale orders only.
         */
        public int ScaleInitLevelSize { get; set; }

        /**
         * @brief Defines the order size of the subsequent scale order components.
         * For Scale orders only. Used in conjunction with scaleInitLevelSize().
         */
        public int ScaleSubsLevelSize { get; set; }

        /**
         * @brief Defines the price increment between scale components.
         * For Scale orders only. This value is compulsory.
         */
        public double ScalePriceIncrement { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended Scale orders.
         */
        public double ScalePriceAdjustValue { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended Scale orders.
         */
        public int ScalePriceAdjustInterval { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended scale orders.
         */
        public double ScaleProfitOffset { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended scale orders.
         */
        public bool ScaleAutoReset { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended scale orders.
         */
        public int ScaleInitPosition { get; set; }

        /**
          * @brief - DOC_TODO
          * For extended scale orders.
          */
        public int ScaleInitFillQty { get; set; }

        /**
         * @brief - DOC_TODO
         * For extended scale orders.
         */
        public bool ScaleRandomPercent { get; set; }

        /**
         * @brief For hedge orders.
         * Possible values include:\n
         *      D - delta \n
         *      B - beta \n
         *      F - FX \n
         *      P - Pair \n
         */
        public string HedgeType { get; set; }

        /**
         * @brief - DOC_TODO
         * Beta = x for Beta hedge orders, ratio = y for Pair hedge order
         */
        public string HedgeParam { get; set; }

        /**
         * @brief The account the trade will be allocated to.
         */
        public string Account { get; set; }

        /**
         * @brief - DOC_TODO
         * Institutions only. Indicates the firm which will settle the trade.
         */
        public string SettlingFirm { get; set; }

        /**
         * @brief Specifies the true beneficiary of the order.
         * For IBExecution customers. This value is required for FUT/FOP orders for reporting to the exchange.
         */
        public string ClearingAccount { get; set; }

        /**
        * @brief For exeuction-only clients to know where do they want their shares to be cleared at.
         * Valid values are: IB, Away, and PTA (post trade allocation).
        */
        public string ClearingIntent { get; set; }

        /**
         * @brief The algorithm strategy.
         * As of API verion 9.6, the following algorithms are supported:\n
         *      ArrivalPx - Arrival Price \n
         *      DarkIce - Dark Ice \n
         *      PctVol - Percentage of Volume \n
         *      Twap - TWAP (Time Weighted Average Price) \n
         *      Vwap - VWAP (Volume Weighted Average Price) \n
         * For more information about IB's API algorithms, refer to https://www.interactivebrokers.com/en/software/api/apiguide/tables/ibalgo_parameters.htm
        */
        public string AlgoStrategy { get; set; }

        /**
        * @brief The list of parameters for the IB algorithm.
         * For more information about IB's API algorithms, refer to https://www.interactivebrokers.com/en/software/api/apiguide/tables/ibalgo_parameters.htm
        */
        public List<TagValue> AlgoParams { get; set; }

        /**
        * @brief Allows to retrieve the commissions and margin information.
         * When placing an order with this attribute set to true, the order will not be placed as such. Instead it will used to request the commissions and margin information that would result from this order.
        */
        public bool WhatIf { get; set; }

        public string AlgoId { get; set; }

        /**
        * @brief Orders routed to IBDARK are tagged as “post only” and are held in IB's order book, where incoming SmartRouted orders from other IB customers are eligible to trade against them.
         * For IBDARK orders only.
        */
        public bool NotHeld { get; set; }

        /**
         * @brief Advanced parameters for Smart combo routing. \n
         * These features are for both guaranteed and nonguaranteed combination orders routed to Smart, and are available based on combo type and order type. 
		 * SmartComboRoutingParams is similar to AlgoParams in that it makes use of tag/value pairs to add parameters to combo orders. \n
		 * Make sure that you fully understand how Advanced Combo Routing works in TWS itself first: https://www.interactivebrokers.com/en/software/tws/usersguidebook/specializedorderentry/advanced_combo_routing.htm \n
		 * The parameters cover the following capabilities:
		 *  - Non-Guaranteed - Determine if the combo order is Guaranteed or Non-Guaranteed. \n
		 *    Tag = NonGuaranteed \n
		 *    Value = 0: The order is guaranteed \n
		 *    Value = 1: The order is non-guaranteed \n
		 * \n
		 *  - Select Leg to Fill First - User can specify which leg to be executed first. \n
		 *    Tag = LeginPrio \n
		 *    Value = -1: No priority is assigned to either combo leg \n
		 *    Value = 0: Priority is assigned to the first leg being added to the comboLeg \n
		 *    Value = 1: Priority is assigned to the second leg being added to the comboLeg \n
		 *    Note: The LeginPrio parameter can only be applied to two-legged combo. \n
		 * \n
		 *  - Maximum Leg-In Combo Size - Specify the maximum allowed leg-in size per segment \n
		 *    Tag = MaxSegSize \n
		 *    Value = Unit of combo size \n
		 * \n
		 *  - Do Not Start Next Leg-In if Previous Leg-In Did Not Finish - Specify whether or not the system should attempt to fill the next segment before the current segment fills. \n
		 *    Tag = DontLeginNext \n
		 *    Value = 0: Start next leg-in even if previous leg-in did not finish \n
		 *    Value = 1: Do not start next leg-in if previous leg-in did not finish \n
		 * \n
		 *  - Price Condition - Combo order will be rejected or cancelled if the leg market price is outside of the specified price range [CondPriceMin, CondPriceMax] \n
		 *    Tag = PriceCondConid: The ContractID of the combo leg to specify price condition on \n
		 *    Value = The ContractID \n
		 *    Tag = CondPriceMin: The lower price range of the price condition \n
		 *    Value = The lower price \n
		 *    Tag = CondPriceMax: The upper price range of the price condition \n
		 *    Value = The upper price \n
		 * \n
         */
        public List<TagValue> SmartComboRoutingParams { get; set; }

        /**
        * @brief List of Per-leg price following the same sequence combo legs are added. The combo price must be left unspecified when using per-leg prices.
        */
        public List<OrderComboLeg> OrderComboLegs { get; set; } = new List<OrderComboLeg>();

        /**
         * @brief - DOC_TODO
         */
        public List<TagValue> OrderMiscOptions { get; set; } = new List<TagValue>();

        /**
         * @brief for GTC orders.
         */
        public string ActiveStartTime { get; set; }

        /**
        * @brief for GTC orders.
        */
        public string ActiveStopTime { get; set; }

        /**
         * @brief Used for scale orders.
         */
        public string ScaleTable { get; set; }

        /**
         * @brief model code
         */
        public string ModelCode { get; set; }

        /**
         * @brief This is a regulartory attribute that applies to all US Commodity (Futures) Exchanges, 
         * provided to allow client to comply with CFTC Tag 50 Rules
         */

        public string ExtOperator { get; set; }

        /**
         * @brief The native cash quantity
         */
        public double CashQty { get; set; }

        /**
         * @brief Identifies a person as the responsible party for investment decisions within the firm. Orders covered by MiFID 2 (Markets in Financial Instruments Directive 2) must include either Mifid2DecisionMaker or Mifid2DecisionAlgo field (but not both). Requires TWS 969+.
         */
        public string Mifid2DecisionMaker { get; set; }

        /**
         * @brief Identifies the algorithm responsible for investment decisions within the firm. Orders covered under MiFID 2 must include either Mifid2DecisionMaker or Mifid2DecisionAlgo, but cannot have both. Requires TWS 969+.
         */
        public string Mifid2DecisionAlgo { get; set; }

        /**
         * @brief For MiFID 2 reporting; identifies a person as the responsible party for the execution of a transaction within the firm. Requires TWS 969+.
         */
        public string Mifid2ExecutionTrader { get; set; }

        /**
         * @brief For MiFID 2 reporting; identifies the algorithm responsible for the execution of a transaction within the firm. Requires TWS 969+.
         */
        public string Mifid2ExecutionAlgo { get; set; }

        /**
         * @brief Don't use auto price for hedge
         */
        public bool DontUseAutoPriceForHedge { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public bool RandomizeSize { get; set; }
        /**
         * @brief - DOC_TODO
         */

        public bool RandomizePrice { get; set; }

        /**
        * @brief Pegged-to-benchmark orders: this attribute will contain the conId of the contract against which the order will be pegged.
        */
        public int ReferenceContractId { get; set; }

        /**
        * @brief Pegged-to-benchmark orders: indicates whether the order's pegged price should increase or decreases.
        */
        public bool IsPeggedChangeAmountDecrease { get; set; }

        /**
        * @brief Pegged-to-benchmark orders: amount by which the order's pegged price should move.
        */
        public double PeggedChangeAmount { get; set; }

        /**
        * @brief Pegged-to-benchmark orders: the amount the reference contract needs to move to adjust the pegged order.
        */
        public double ReferenceChangeAmount { get; set; }

        /**
        * @brief Pegged-to-benchmark orders: the exchange against which we want to observe the reference contract.
        */
        public string ReferenceExchange { get; set; }

        /**
        * @brief Adjusted Stop orders: the parent order will be adjusted to the given type when the adjusted trigger price is penetrated.
        */
        public string AdjustedOrderType { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public double TriggerPrice { get; set; }

        /**
         * @brief - DOC_TODO
         */
        public double LmtPriceOffset { get; set; }

        /**
        * @brief Adjusted Stop orders: specifies the stop price of the adjusted (STP) parent
        */
        public double AdjustedStopPrice { get; set; }

        /**
        * @brief Adjusted Stop orders: specifies the stop limit price of the adjusted (STPL LMT) parent
        */
        public double AdjustedStopLimitPrice { get; set; }

        /**
        * @brief Adjusted Stop orders: specifies the trailing amount of the adjusted (TRAIL) parent 
        */
        public double AdjustedTrailingAmount { get; set; }

        /**
         * @brief Adjusted Stop orders: specifies where the trailing unit is an amount (set to 0) or a percentage (set to 1)
         */
        public int AdjustableTrailingUnit { get; set; }

        /**
       * @brief Conditions determining when the order will be activated or canceled 
       */
        public List<OrderCondition> Conditions { get; set; }

        /**
        * @brief Indicates whether or not conditions will also be valid outside Regular Trading Hours
        */
        public bool ConditionsIgnoreRth { get; set; }

        /**
        * @brief Conditions can determine if an order should become active or canceled.
        */
        public bool ConditionsCancelOrder { get; set; }

        /**
        * @brief Define the Soft Dollar Tier used for the order. Only provided for registered professional advisors and hedge and mutual funds.
        */
        public SoftDollarTier Tier { get; set; }

        // Note: Two orders can be 'equivalent' even if all fields do not match. This function is not intended to be used with Order objects returned from TWS.
        public override bool Equals(object pOther)
        {
            if (this == pOther)
            {
                return true;
            }

            if (pOther == null)
            {
                return false;
            }

            var lTheOther = (Order)pOther;

            if (PermId == lTheOther.PermId)
            {
                return true;
            }

            if (OrderId != lTheOther.OrderId ||
                ClientId != lTheOther.ClientId ||
                TotalQuantity != lTheOther.TotalQuantity ||
                LmtPrice != lTheOther.LmtPrice ||
                AuxPrice != lTheOther.AuxPrice ||
                OcaType != lTheOther.OcaType ||
                Transmit != lTheOther.Transmit ||
                ParentId != lTheOther.ParentId ||
                BlockOrder != lTheOther.BlockOrder ||
                SweepToFill != lTheOther.SweepToFill ||
                DisplaySize != lTheOther.DisplaySize ||
                TriggerMethod != lTheOther.TriggerMethod ||
                OutsideRth != lTheOther.OutsideRth ||
                Hidden != lTheOther.Hidden ||
                OverridePercentageConstraints != lTheOther.OverridePercentageConstraints ||
                AllOrNone != lTheOther.AllOrNone ||
                MinQty != lTheOther.MinQty ||
                PercentOffset != lTheOther.PercentOffset ||
                TrailStopPrice != lTheOther.TrailStopPrice ||
                TrailingPercent != lTheOther.TrailingPercent ||
                Origin != lTheOther.Origin ||
                ShortSaleSlot != lTheOther.ShortSaleSlot ||
                DiscretionaryAmt != lTheOther.DiscretionaryAmt ||
                ETradeOnly != lTheOther.ETradeOnly ||
                FirmQuoteOnly != lTheOther.FirmQuoteOnly ||
                NbboPriceCap != lTheOther.NbboPriceCap ||
                OptOutSmartRouting != lTheOther.OptOutSmartRouting ||
                AuctionStrategy != lTheOther.AuctionStrategy ||
                StartingPrice != lTheOther.StartingPrice ||
                StockRefPrice != lTheOther.StockRefPrice ||
                Delta != lTheOther.Delta ||
                StockRangeLower != lTheOther.StockRangeLower ||
                StockRangeUpper != lTheOther.StockRangeUpper ||
                Volatility != lTheOther.Volatility ||
                VolatilityType != lTheOther.VolatilityType ||
                ContinuousUpdate != lTheOther.ContinuousUpdate ||
                ReferencePriceType != lTheOther.ReferencePriceType ||
                DeltaNeutralAuxPrice != lTheOther.DeltaNeutralAuxPrice ||
                DeltaNeutralConId != lTheOther.DeltaNeutralConId ||
                DeltaNeutralShortSale != lTheOther.DeltaNeutralShortSale ||
                DeltaNeutralShortSaleSlot != lTheOther.DeltaNeutralShortSaleSlot ||
                BasisPoints != lTheOther.BasisPoints ||
                BasisPointsType != lTheOther.BasisPointsType ||
                ScaleInitLevelSize != lTheOther.ScaleInitLevelSize ||
                ScaleSubsLevelSize != lTheOther.ScaleSubsLevelSize ||
                ScalePriceIncrement != lTheOther.ScalePriceIncrement ||
                ScalePriceAdjustValue != lTheOther.ScalePriceAdjustValue ||
                ScalePriceAdjustInterval != lTheOther.ScalePriceAdjustInterval ||
                ScaleProfitOffset != lTheOther.ScaleProfitOffset ||
                ScaleAutoReset != lTheOther.ScaleAutoReset ||
                ScaleInitPosition != lTheOther.ScaleInitPosition ||
                ScaleInitFillQty != lTheOther.ScaleInitFillQty ||
                ScaleRandomPercent != lTheOther.ScaleRandomPercent ||
                WhatIf != lTheOther.WhatIf ||
                NotHeld != lTheOther.NotHeld ||
                ExemptCode != lTheOther.ExemptCode ||
                RandomizePrice != lTheOther.RandomizePrice ||
                RandomizeSize != lTheOther.RandomizeSize ||
                Solicited != lTheOther.Solicited ||
                ConditionsIgnoreRth != lTheOther.ConditionsIgnoreRth ||
                ConditionsCancelOrder != lTheOther.ConditionsCancelOrder ||
                Tier != lTheOther.Tier ||
                CashQty != lTheOther.CashQty ||
                DontUseAutoPriceForHedge != lTheOther.DontUseAutoPriceForHedge)
            {
                return false;
            }

            if (Util.StringCompare(Action, lTheOther.Action) != 0 ||
                Util.StringCompare(OrderType, lTheOther.OrderType) != 0 ||
                Util.StringCompare(Tif, lTheOther.Tif) != 0 ||
                Util.StringCompare(ActiveStartTime, lTheOther.ActiveStartTime) != 0 ||
                Util.StringCompare(ActiveStopTime, lTheOther.ActiveStopTime) != 0 ||
                Util.StringCompare(OcaGroup, lTheOther.OcaGroup) != 0 ||
                Util.StringCompare(OrderRef, lTheOther.OrderRef) != 0 ||
                Util.StringCompare(GoodAfterTime, lTheOther.GoodAfterTime) != 0 ||
                Util.StringCompare(GoodTillDate, lTheOther.GoodTillDate) != 0 ||
                Util.StringCompare(Rule80A, lTheOther.Rule80A) != 0 ||
                Util.StringCompare(FaGroup, lTheOther.FaGroup) != 0 ||
                Util.StringCompare(FaProfile, lTheOther.FaProfile) != 0 ||
                Util.StringCompare(FaMethod, lTheOther.FaMethod) != 0 ||
                Util.StringCompare(FaPercentage, lTheOther.FaPercentage) != 0 ||
                Util.StringCompare(OpenClose, lTheOther.OpenClose) != 0 ||
                Util.StringCompare(DesignatedLocation, lTheOther.DesignatedLocation) != 0 ||
                Util.StringCompare(DeltaNeutralOrderType, lTheOther.DeltaNeutralOrderType) != 0 ||
                Util.StringCompare(DeltaNeutralSettlingFirm, lTheOther.DeltaNeutralSettlingFirm) != 0 ||
                Util.StringCompare(DeltaNeutralClearingAccount, lTheOther.DeltaNeutralClearingAccount) != 0 ||
                Util.StringCompare(DeltaNeutralClearingIntent, lTheOther.DeltaNeutralClearingIntent) != 0 ||
                Util.StringCompare(DeltaNeutralOpenClose, lTheOther.DeltaNeutralOpenClose) != 0 ||
                Util.StringCompare(DeltaNeutralDesignatedLocation, lTheOther.DeltaNeutralDesignatedLocation) != 0 ||
                Util.StringCompare(HedgeType, lTheOther.HedgeType) != 0 ||
                Util.StringCompare(HedgeParam, lTheOther.HedgeParam) != 0 ||
                Util.StringCompare(Account, lTheOther.Account) != 0 ||
                Util.StringCompare(SettlingFirm, lTheOther.SettlingFirm) != 0 ||
                Util.StringCompare(ClearingAccount, lTheOther.ClearingAccount) != 0 ||
                Util.StringCompare(ClearingIntent, lTheOther.ClearingIntent) != 0 ||
                Util.StringCompare(AlgoStrategy, lTheOther.AlgoStrategy) != 0 ||
                Util.StringCompare(AlgoId, lTheOther.AlgoId) != 0 ||
                Util.StringCompare(ScaleTable, lTheOther.ScaleTable) != 0 ||
                Util.StringCompare(ModelCode, lTheOther.ModelCode) != 0 ||
                Util.StringCompare(ExtOperator, lTheOther.ExtOperator) != 0)
            {
                return false;
            }

            if (!Util.VectorEqualsUnordered(AlgoParams, lTheOther.AlgoParams))
            {
                return false;
            }

            if (!Util.VectorEqualsUnordered(SmartComboRoutingParams, lTheOther.SmartComboRoutingParams))
            {
                return false;
            }

            // compare order combo legs
            if (!Util.VectorEqualsUnordered(OrderComboLegs, lTheOther.OrderComboLegs))
            {
                return false;
            }

            return true;
        }
    }
}