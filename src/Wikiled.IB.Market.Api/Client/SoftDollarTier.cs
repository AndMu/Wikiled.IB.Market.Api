namespace Wikiled.IB.Market.Api.Client
{
    /**
     * @class SoftDollarTier
     * @brief A container for storing Soft Dollar Tier information
     */
    public class SoftDollarTier
    {
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

        public override bool Equals(object obj)
        {
            var b = obj as SoftDollarTier;

            if (Equals(b, null))
            {
                return false;
            }

            return string.Compare(Name, b.Name, true) == 0 && string.Compare(Value, b.Value, true) == 0;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Value.GetHashCode();
        }

        public static bool operator ==(SoftDollarTier left, SoftDollarTier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SoftDollarTier left, SoftDollarTier right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}