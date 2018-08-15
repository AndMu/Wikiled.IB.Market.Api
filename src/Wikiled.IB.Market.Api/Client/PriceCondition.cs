using System.Globalization;
using System.IO;
using System.Linq;

namespace Wikiled.IB.Market.Api.Client
{
/** 
 *  @brief Used with conditional orders to cancel or submit order based on price of an instrument. 
 */

    public class PriceCondition : ContractCondition
    {
        protected override string Value
        {
            get => Price.ToString();
            set => Price = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        public double Price { get; set; }
        public TriggerMethod TriggerMethod { get; set; }

        public override string ToString()
        {
            return TriggerMethod.ToFriendlyString() + " " + base.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PriceCondition;

            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) && TriggerMethod == other.TriggerMethod;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + TriggerMethod.GetHashCode();
        }

        public override void Deserialize(IDecoder inStream)
        {
            base.Deserialize(inStream);

            TriggerMethod = (TriggerMethod)inStream.ReadInt();
        }

        public override void Serialize(BinaryWriter outStream)
        {
            base.Serialize(outStream);
            outStream.AddParameter((int)TriggerMethod);
        }

        protected override bool TryParse(string cond)
        {
            var fName = CTriggerMethod.FriendlyNames.Where(n => cond.StartsWith(n))
                .OrderByDescending(n => n.Length)
                .FirstOrDefault();

            if (fName == null)
            {
                return false;
            }

            try
            {
                TriggerMethod = CTriggerMethod.FromFriendlyString(fName);
                cond = cond.Substring(cond.IndexOf(fName) + fName.Length + 1);

                return base.TryParse(cond);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}