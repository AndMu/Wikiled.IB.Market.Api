using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Wikiled.IB.Market.Api.Client
{
    public static class IbParamsList
    {
        public static void AddParameter(this BinaryWriter source, OutgoingMessages msgId)
        {
            AddParameter(source, (int)msgId);
        }

        public static void AddParameter(this BinaryWriter source, int value)
        {
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AddParameter(this BinaryWriter source, double value)
        {
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AddParameter(this BinaryWriter source, bool value)
        {
            if (value)
            {
                AddParameter(source, "1");
            }
            else
            {
                AddParameter(source, "0");
            }
        }

        public static void AddParameter(this BinaryWriter source, string value)
        {
            if (value != null)
            {
                source.Write(Encoding.UTF8.GetBytes(value));
            }

            source.Write(Constants.Eol);
        }

        public static void AddParameter(this BinaryWriter source, Contract value)
        {
            source.AddParameter(value.ConId);
            source.AddParameter(value.Symbol);
            source.AddParameter(value.SecType);
            source.AddParameter(value.LastTradeDateOrContractMonth);
            source.AddParameter(value.Strike);
            source.AddParameter(value.Right);
            source.AddParameter(value.Multiplier);
            source.AddParameter(value.Exchange);
            source.AddParameter(value.PrimaryExch);
            source.AddParameter(value.Currency);
            source.AddParameter(value.LocalSymbol);
            source.AddParameter(value.TradingClass);
            source.AddParameter(value.IncludeExpired);
        }

        public static void AddParameter(this BinaryWriter source, List<TagValue> options)
        {
            var tagValuesStr = new StringBuilder();
            var tagValuesCount = options == null ? 0 : options.Count;

            for (var i = 0; i < tagValuesCount; i++)
            {
                var tagValue = options[i];
                tagValuesStr.Append(tagValue.Tag).Append("=").Append(tagValue.Value).Append(";");
            }

            source.AddParameter(tagValuesStr.ToString());
        }

        public static void AddParameterMax(this BinaryWriter source, double value)
        {
            if (value == double.MaxValue)
            {
                source.Write(Constants.Eol);
            }
            else
            {
                source.AddParameter(value);
            }
        }

        public static void AddParameterMax(this BinaryWriter source, int value)
        {
            if (value == int.MaxValue)
            {
                source.Write(Constants.Eol);
            }
            else
            {
                source.AddParameter(value);
            }
        }
    }
}