namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class OrderComboLeg
     * @brief Allows to specify a price on an order's leg
     * @sa Order, ComboLeg
     */
    public class OrderComboLeg
    {
        public OrderComboLeg()
        {
            Price = double.MaxValue;
        }

        public OrderComboLeg(double pPrice)
        {
            Price = pPrice;
        }

        /**
         * @brief The order's leg's price
         */
        public double Price { get; set; }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            var theOther = (OrderComboLeg)other;

            if (Price != theOther.Price)
            {
                return false;
            }

            return true;
        }
    }
}