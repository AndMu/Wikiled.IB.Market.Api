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

        protected bool Equals(OrderComboLeg other)
        {
            return Price.Equals(other.Price);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((OrderComboLeg)obj);
        }

        public override int GetHashCode()
        {
            return Price.GetHashCode();
        }
    }
}