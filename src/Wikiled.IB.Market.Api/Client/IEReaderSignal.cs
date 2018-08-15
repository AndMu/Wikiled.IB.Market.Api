namespace Wikiled.IB.Market.Api.Client
{
    /**
    * @brief Notifies the thread reading information from the TWS whenever there are messages ready to be consumed. Not currently used in Python API.
    */
    public interface IEReaderSignal
    {
        /**
         * @brief Issues a signal to the consuming thread when there are things to be consumed.
         */
        void IssueSignal();

        /**
         * @brief Makes the consuming thread waiting until a signal is issued.
         */
        void WaitForSignal();
    }
}