namespace Wikiled.IB.Market.Api.Client
{
    public class EMessage
    {
        private readonly byte[] buf;

        public EMessage(byte[] buf)
        {
            this.buf = buf;
        }

        public byte[] GetBuf()
        {
            return buf;
        }
    }
}