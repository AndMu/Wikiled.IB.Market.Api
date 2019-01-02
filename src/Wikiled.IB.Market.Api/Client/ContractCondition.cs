using System;
using System.IO;

namespace Wikiled.IB.Market.Api.Client
{
    public abstract class ContractCondition : OperatorCondition
    {
        private const string Delimiter = " of ";

        public ContractCondition()
        {
            ContractResolver = (conid, exch) => conid + "(" + exch + ")";
        }

        public int ConId { get; set; }

        public string Exchange { get; set; }

        public Func<int, string, string> ContractResolver { get; set; }

        public override string ToString()
        {
            return Type + Delimiter + ContractResolver(ConId, Exchange) + base.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ContractCondition;

            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) && ConId == other.ConId && Exchange.Equals(other.Exchange);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + ConId.GetHashCode() + Exchange.GetHashCode();
        }

        protected override bool TryParse(string cond)
        {
            try
            {
                if (cond.Substring(0, cond.IndexOf(Delimiter)) != Type.ToString())
                {
                    return false;
                }

                cond = cond.Substring(cond.IndexOf(Delimiter) + Delimiter.Length);
                int conid;

                if (!int.TryParse(cond.Substring(0, cond.IndexOf("(")), out conid))
                {
                    return false;
                }

                ConId = conid;
                cond = cond.Substring(cond.IndexOf("(") + 1);
                Exchange = cond.Substring(0, cond.IndexOf(")"));
                cond = cond.Substring(cond.IndexOf(")") + 1);

                return base.TryParse(cond);
            }
            catch
            {
                return false;
            }
        }

        public override void Deserialize(IDecoder inStream)
        {
            base.Deserialize(inStream);

            ConId = inStream.ReadInt();
            Exchange = inStream.ReadString();
        }

        public override void Serialize(BinaryWriter outStream)
        {
            base.Serialize(outStream);
            outStream.AddParameter(ConId);
            outStream.AddParameter(Exchange);
        }
    }
}