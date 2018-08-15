namespace Wikiled.IB.Market.Api.Client
{
    internal class StringSuffixParser
    {
        public StringSuffixParser(string str)
        {
            Rest = str;
        }

        public string Rest { get; private set; }

        private string SkipSuffix(string perfix)
        {
            return Rest.Substring(Rest.IndexOf(perfix) + perfix.Length);
        }

        public string GetNextSuffixedValue(string perfix)
        {
            var rval = Rest.Substring(0, Rest.IndexOf(perfix));
            Rest = SkipSuffix(perfix);

            return rval;
        }
    }
}