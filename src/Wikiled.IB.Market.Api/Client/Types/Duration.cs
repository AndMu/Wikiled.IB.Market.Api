using System;

namespace Wikiled.IB.Market.Api.Client.Types
{
    public class Duration
    {
        private string text;

        public Duration(int length, DurationType type)
        {
            Length = length;
            Type = type;

            switch (Type)
            {
                case DurationType.Seconds:
                    text = "S";
                    break;
                case DurationType.Days:
                    text = "D";
                    break;
                case DurationType.Weeks:
                    text = "W";
                    break;
                case DurationType.Months:
                    text = "M";
                    break;
                case DurationType.Years:
                    text = "Y";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            text = $"{Length} {text}";
        }

        public DurationType Type { get; }

        public int Length { get; }

        public override string ToString()
        {
            return text;
        }
    }
}
