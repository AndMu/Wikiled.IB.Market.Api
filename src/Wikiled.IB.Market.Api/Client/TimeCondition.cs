namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Time condition used in conditional orders to submit or cancel orders at specified time. 
    */
    public class TimeCondition : OperatorCondition
    {
        private const string Header = "time";

        protected override string Value
        {
            get => Time;
            set => Time = value;
        }

        /**
        * @brief Time field used in conditional order logic. Valid format: YYYYMMDD HH:MM:SS
        */

        public string Time { get; set; }

        public override string ToString()
        {
            return Header + base.ToString();
        }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(Header))
            {
                return false;
            }

            cond = cond.Replace(Header, "");
            return base.TryParse(cond);
        }
    }
}