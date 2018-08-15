namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class ComboLeg
     * @brief Class representing a leg within combo orders.
     * @sa Order
     */
    public class ComboLeg
    {
        public static int Same = 0;
        public static int Open = 1;
        public static int Close = 2;
        public static int Unknown = 3;


        public ComboLeg()
        {
        }

        public ComboLeg(int conId,
                        int ratio,
                        string action,
                        string exchange,
                        int openClose,
                        int shortSaleSlot,
                        string designatedLocation,
                        int exemptCode)
        {
            ConId = conId;
            Ratio = ratio;
            Action = action;
            Exchange = exchange;
            OpenClose = openClose;
            ShortSaleSlot = shortSaleSlot;
            DesignatedLocation = designatedLocation;
            ExemptCode = exemptCode;
        }

        /**
         * @brief The Contract's IB's unique id
         */
        public int ConId { get; set; }

        /**
          * @brief Select the relative number of contracts for the leg you are constructing. To help determine the ratio for a specific combination order, refer to the Interactive Analytics section of the User's Guide.
          */
        public int Ratio { get; set; }

        /**
         * @brief The side (buy or sell) of the leg:\n
         *      - For individual accounts, only BUY and SELL are available. SSHORT is for institutions.
         */
        public string Action { get; set; }

        /**
         * @brief The destination exchange to which the order will be routed.
         */
        public string Exchange { get; set; }

        /**
        * @brief Specifies whether an order is an open or closing order.
        * For instituational customers to determine if this order is to open or close a position.
        *      0 - Same as the parent security. This is the only option for retail customers.\n
        *      1 - Open. This value is only valid for institutional customers.\n
        *      2 - Close. This value is only valid for institutional customers.\n
        *      3 - Unknown
        */
        public int OpenClose { get; set; }

        /**
         * @brief For stock legs when doing short selling.
         * Set to 1 = clearing broker, 2 = third party
         */
        public int ShortSaleSlot { get; set; }

        /**
         * @brief When ShortSaleSlot is 2, this field shall contain the designated location.
         */
        public string DesignatedLocation { get; set; }

        /**
         * @brief DOC_TODO
         */
        public int ExemptCode { get; set; }
    }
}