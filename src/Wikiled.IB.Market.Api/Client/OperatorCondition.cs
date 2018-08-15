﻿using System.IO;

namespace Wikiled.IB.Market.Api.Client
{
    public abstract class OperatorCondition : OrderCondition
    {
        private const string Header = " is ";
        protected abstract string Value { get; set; }
        public bool IsMore { get; set; }

        public override string ToString()
        {
            return Header + (IsMore ? ">= " : "<= ") + Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as OperatorCondition;

            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) && Value.Equals(other.Value) && IsMore == other.IsMore;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Value.GetHashCode() + IsMore.GetHashCode();
        }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(Header))
            {
                return false;
            }

            try
            {
                cond = cond.Replace(Header, "");

                if (!cond.StartsWith(">=") && !cond.StartsWith("<="))
                {
                    return false;
                }

                IsMore = cond.StartsWith(">=");

                if (base.TryParse(cond.Substring(cond.LastIndexOf(" "))))
                {
                    cond = cond.Substring(0, cond.LastIndexOf(" "));
                }

                Value = cond.Substring(3);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override void Deserialize(IDecoder inStream)
        {
            base.Deserialize(inStream);

            IsMore = inStream.ReadBoolFromInt();
            Value = inStream.ReadString();
        }

        public override void Serialize(BinaryWriter outStream)
        {
            base.Serialize(outStream);
            outStream.AddParameter(IsMore);
            outStream.AddParameter(Value);
        }
    }
}