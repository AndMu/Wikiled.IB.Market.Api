namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class FamilyCodesMessage
    {
        public FamilyCode[] FamilyCodes { get; }

        public FamilyCodesMessage(FamilyCode[] familyCodes)
        {
            FamilyCodes = familyCodes;
        }
    }
}
