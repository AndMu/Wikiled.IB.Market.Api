using NUnit.Framework;
using System;
using Wikiled.IB.Market.Api.Client.Helpers;

namespace Wikiled.IB.Market.Api.Tests.Client.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests
    {
        [TestCase(10, 15, "Eastern Standard Time")]
        public void Construct(int hours, int result, string zone)
        {
            Assert.AreEqual(result, DateTimeHelper.ToUtc(new DateTime(2012, 02, 02, hours, 0, 0), TimeZoneInfo.FindSystemTimeZoneById(zone)).Hour);
        }
    }
}