using System.Collections.Generic;

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

        protected bool Equals(TagValue other)
        {
            return Tag == other.Tag && Value == other.Value;
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

            return Equals((TagValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Tag != null ? Tag.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
    }
}