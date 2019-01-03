using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.IB.Market.Api.Client;
using Wikiled.IB.Market.Api.Client.Messages;
using Wikiled.IB.Market.Api.Client.Serialization;

namespace Wikiled.IB.Market.Api.Tests.Client.Serialization
{
    [TestFixture]
    public class CsvSerializerTests
    {
        private Mock<IClientWrapper> mockClientWrapper;

        private CsvSerializer instance;

        private TestScheduler scheduler;

        private string fileName;

        [SetUp]
        public void SetUp()
        {
            fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "stock.csv");
            scheduler = new TestScheduler();
            mockClientWrapper = new Mock<IClientWrapper>();
            mockClientWrapper.Setup(item => item.TimeZone).Returns(TimeZoneInfo.Utc);
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvSerializer(null));
        }

        [Test]
        public async Task Save()
        {
            var stream = scheduler.CreateColdObservable(
                new Recorded<Notification<Bar>>(100,
                                                Notification.CreateOnNext(
                                                    new Bar("20170101", 2, 2, 1, 3, 100, 10, 2))),
                new Recorded<Notification<Bar>>(200,
                                                Notification.CreateOnNext(
                                                    new Bar("20170101", 2, 2, 1, 3, 100, 10, 2))),
                new Recorded<Notification<Bar>>(300,
                                                Notification.CreateOnNext(
                                                    new Bar("20170101", 2, 2, 1, 3, 100, 10, 2))),
                new Recorded<Notification<Bar>>(400,
                                                Notification.CreateOnCompleted<Bar>()));
            var saveTask = instance.Save(fileName, stream);

            for (int i = 0; i < 4; i++)
            {
                await Task.Delay(100).ConfigureAwait(false);
                scheduler.AdvanceBy(100);
            }

            await Task.Delay(100).ConfigureAwait(false);
            Assert.IsTrue(saveTask.IsCompleted);
            var lines = File.ReadAllLines(fileName);
            Assert.AreEqual(4, lines.Length);
        }

        private CsvSerializer CreateInstance()
        {
            return new CsvSerializer(mockClientWrapper.Object);
        }
    }
}
