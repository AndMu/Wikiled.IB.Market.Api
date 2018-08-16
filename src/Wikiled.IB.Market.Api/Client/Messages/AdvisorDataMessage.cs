namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class AdvisorDataMessage 
    {
        public AdvisorDataMessage(int faDataType, string data)
        {
            FaDataType = faDataType;
            Data = data;
        }

        public int FaDataType { get; set; }

        public string Data { get; set; }
    }
}
