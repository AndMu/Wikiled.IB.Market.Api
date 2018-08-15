namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @class TagValue
    * @brief Convenience class to define key-value pairs
    */
    public class TagValue
    {
        public TagValue()
        {
        }

        public TagValue(string pTag, string pValue)
        {
            Tag = pTag;
            Value = pValue;
        }

        public string Tag { get; set; }


        public string Value { get; set; }

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

            var lTheOther = (TagValue)other;

            if (Util.StringCompare(Tag, lTheOther.Tag) != 0 ||
                Util.StringCompare(Value, lTheOther.Value) != 0)
            {
                return false;
            }

            return true;
        }
    }
}