using System;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class ErrorManager : IDisposable
    {
        private readonly ILogger logger;

        private readonly IBClient ibClient;

        public ErrorManager(IBClient ibClient, ILoggerFactory loggerFactory)
        {
            this.ibClient = ibClient ?? throw new ArgumentNullException(nameof(ibClient));
            logger = loggerFactory?.CreateLogger(GetType()) ?? throw new ArgumentNullException(nameof(logger));
            ibClient.Error += ErrorHandler;
        }

        private void ErrorHandler(IErrorDescription message)
        {
            if (message is ExceptionDescription exceptionDescription)
            {
                logger.LogError(exceptionDescription.Exception, "Exception");
            }
            else
            {
                if (message.ErrorCode == 2104 ||
                    message.ErrorCode == 2106)
                {
                    logger.LogDebug(message.ToString());
                }
                else
                {
                    logger.LogError(message.ToString());
                }
            }
        }

        public void Dispose()
        {
            ibClient.Error -= ErrorHandler;
        }
    }
}
