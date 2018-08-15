using System;

namespace Wikiled.IB.Market.Api.Client
{
    public static class CTriggerMethod
    {
        public static readonly string[] FriendlyNames =
        {
            "default",
            "double bid/ask",
            "last",
            "double last",
            "bid/ask",
            "",
            "",
            "last of bid/ask",
            "mid-point"
        };


        public static string ToFriendlyString(this TriggerMethod th)
        {
            return FriendlyNames[(int)th];
        }

        public static TriggerMethod FromFriendlyString(string friendlyName)
        {
            return (TriggerMethod)Array.IndexOf(FriendlyNames, friendlyName);
        }
    }
}