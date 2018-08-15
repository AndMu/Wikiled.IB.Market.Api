namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @class MarginCondition
    * @brief This class represents a condition requiring the margin cushion reaching a given percent to be fulfilled.
    * Orders can be activated or canceled if a set of given conditions is met. A MarginCondition is met whenever the margin penetrates the given percent.
    */
    public class MarginCondition : OperatorCondition
    {
        private const string Header = "the margin cushion percent";

        protected override string Value
        {
            get => Percent.ToString();
            set => Percent = int.Parse(value);
        }

        /**
        * @brief Margin percent to trigger condition.
        */
        public int Percent { get; set; }

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