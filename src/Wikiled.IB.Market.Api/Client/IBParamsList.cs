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

        public static void AddParameter<T>(this BinaryWriter source, T? value)
            where T : struct
        {
            AddParameter(source, value == null ? null : value.ToString());
        }

        public static void AddParameter(this BinaryWriter source, int value)
        {
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AddParameter(this BinaryWriter source, double value)
        {
            AddParameter(source, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AddParameter(this BinaryWriter source, bool? value)
        {
            if (value.HasValue)
            {
                AddParameter(source, value.Value ? "1" : "0");
            }
            else
            {
                source.Write(Constants.Eol);
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
            source.AddParameter(value.SecType.ToString());
            source.AddParameter(value.LastTradeDateOrContractMonth);
            source.AddParameter(value.Strike);
            source.AddParameter(value.Right);
            source.AddParameter(value.Multiplier);
            source.AddParameter(value.Exchange.ToString());
            source.AddParameter(value.PrimaryExch);
            source.AddParameter(value.Currency);
            source.AddParameter(value.LocalSymbol);
            source.AddParameter(value.TradingClass);
            source.AddParameter(value.IncludeExpired);
        }

        public static void AddParameter(this BinaryWriter source, List<TagValue> options)
        {
            source.AddParameter(Util.TagValueListToString(options));
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