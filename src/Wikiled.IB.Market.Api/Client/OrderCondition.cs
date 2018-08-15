using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Wikiled.IB.Market.Api.Client
{
    [ComVisible(true)]
    public abstract class OrderCondition
    {
        public OrderConditionType Type { get; private set; }
        public bool IsConjunctionConnection { get; set; }

        public static OrderCondition Create(OrderConditionType type)
        {
            OrderCondition rval = null;

            switch (type)
            {
                case OrderConditionType.Execution:
                    rval = new ExecutionCondition();
                    break;

                case OrderConditionType.Margin:
                    rval = new MarginCondition();
                    break;

                case OrderConditionType.PercentCange:
                    rval = new PercentChangeCondition();
                    break;

                case OrderConditionType.Price:
                    rval = new PriceCondition();
                    break;

                case OrderConditionType.Time:
                    rval = new TimeCondition();
                    break;

                case OrderConditionType.Volume:
                    rval = new VolumeCondition();
                    break;
            }

            if (rval != null)
            {
                rval.Type = type;
            }

            return rval;
        }

        public virtual void Serialize(BinaryWriter outStream)
        {
            outStream.AddParameter(IsConjunctionConnection ? "a" : "o");
        }

        public virtual void Deserialize(IDecoder inStream)
        {
            IsConjunctionConnection = inStream.ReadString() == "a";
        }

        protected virtual bool TryParse(string cond)
        {
            IsConjunctionConnection = cond == " and";

            return IsConjunctionConnection || cond == " or";
        }

        public static OrderCondition Parse(string cond)
        {
            var conditions = Enum.GetValues(typeof(OrderConditionType))
                .OfType<OrderConditionType>()
                .Select(t => Create(t))
                .ToList();

            return conditions.FirstOrDefault(c => c.TryParse(cond));
        }

        public override bool Equals(object obj)
        {
            var other = obj as OrderCondition;

            if (other == null)
            {
                return false;
            }

            return IsConjunctionConnection == other.IsConjunctionConnection && Type == other.Type;
        }

        public override int GetHashCode()
        {
            return IsConjunctionConnection.GetHashCode() + Type.GetHashCode();
        }
    }
}