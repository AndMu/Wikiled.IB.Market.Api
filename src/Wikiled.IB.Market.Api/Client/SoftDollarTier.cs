namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class SoftDollarTier
     * @brief A container for storing Soft Dollar Tier information
     */
    public class SoftDollarTier
    {
        public SoftDollarTier()
            : this(null, null, null)
        {
        }

        public SoftDollarTier(string name, string value, string displayName)
        {
            Name = name;
            Value = value;
            DisplayName = displayName;
        }

        /**
         * @brief The name of the Soft Dollar Tier
         */
        public string Name { get; }

        /**
         * @brief The value of the Soft Dollar Tier
         */
        public string Value { get; }

        /**
         * @brief The display name of the Soft Dollar Tier
         */
        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }

        protected bool Equals(SoftDollarTier other)
        {
            return Name == other.Name && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SoftDollarTier)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(SoftDollarTier left, SoftDollarTier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SoftDollarTier left, SoftDollarTier right)
        {
            return !Equals(left, right);
        }
    }
}