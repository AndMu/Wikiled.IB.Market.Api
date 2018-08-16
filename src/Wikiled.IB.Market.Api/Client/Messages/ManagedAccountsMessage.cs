using System.Collections.Generic;

namespace Wikiled.IB.Market.Api.Client.Messages
{
    public class ManagedAccountsMessage
    {
        public ManagedAccountsMessage(string managedAccounts)
        {
            ManagedAccounts = new List<string>(managedAccounts.Split(','));
        }

        public List<string> ManagedAccounts { get; set; }
    }
}
