namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class FamilyCodesMessage
    {
        public FamilyCode[] FamilyCodes { get; private set; }

        public FamilyCodesMessage(FamilyCode[] familyCodes)
        {
            this.FamilyCodes = familyCodes;
        }
    }
}
