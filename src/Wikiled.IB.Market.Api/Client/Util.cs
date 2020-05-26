using System;
using System.Collections.Generic;
using System.Text;

namespace Wikiled.IB.Market.Api.Client
{
    public static class Util
    {
        public static bool StringIsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }


        public static string NormalizeString(string str)
        {
            return str ?? "";
        }

        public static int StringCompare(string lhs, string rhs)
        {
            return NormalizeString(lhs).CompareTo(NormalizeString(rhs));
        }

        public static bool VectorEqualsUnordered<T>(List<T> lhs, List<T> rhs)
        {
            if (lhs == rhs)
            {
                return true;
            }

            var lhsCount = lhs?.Count ?? 0;
            var rhsCount = rhs?.Count ?? 0;

            if (lhsCount != rhsCount)
            {
                return false;
            }

            if (lhsCount == 0)
            {
                return true;
            }

            var matchedRhsElems = new bool[rhsCount];

            for (var lhsIdx = 0; lhsIdx < lhsCount; ++lhsIdx)
            {
                object lhsElem = lhs[lhsIdx];
                var rhsIdx = 0;
                for (; rhsIdx < rhsCount; ++rhsIdx)
                {
                    if (matchedRhsElems[rhsIdx])
                    {
                        continue;
                    }

                    if (lhsElem.Equals(rhs[rhsIdx]))
                    {
                        matchedRhsElems[rhsIdx] = true;
                        break;
                    }
                }

                if (rhsIdx >= rhsCount)
                {
                    // no matching elem found
                    return false;
                }
            }

            return true;
        }

        public static string IntMaxString(int value)
        {
            return value == int.MaxValue ? "" : "" + value;
        }

        public static string DoubleMaxString(double value)
        {
            return value == double.MaxValue ? "" : "" + value;
        }

        public static string UnixSecondsToString(long seconds, string format)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(seconds)).ToString(format);
        }

        public static string FormatDoubleString(string str)
        {
            return string.IsNullOrEmpty(str) ? "" : $"{double.Parse(str),0:N2}";
        }

        public static string LongMaxString(long value)
        {
            return (value == Int64.MaxValue) ? "" : "" + value;
        }

        public static string TagValueListToString(List<TagValue> options)
        {
            StringBuilder tagValuesStr = new StringBuilder();
            int tagValuesCount = options?.Count ?? 0;

            for (int i = 0; i < tagValuesCount; i++)
            {
                TagValue tagValue = options[i];
                tagValuesStr.Append(tagValue.Tag).Append("=").Append(tagValue.Value).Append(";");
            }

            return tagValuesStr.ToString();
        }

    }
}