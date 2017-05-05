using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Loki.MSSQL;
using Loki.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace Loki.Tests
{
    [TestClass]
    public class SimpleTest
    {
        private TestClient _testClient;
        private TestClient _testClient2;

        [TestInitialize]
        public void Initialize()
        {
            _testClient = new TestClient();
            _testClient2 = new TestClient();

            Fixture fixture = new Fixture();
            fixture.Register(() => false);

            Source.DummyItems.AddRange(fixture.CreateMany<DummyItem>(200));

            List<EndPoint> redisEndPoints = new List<EndPoint>
            {
                new DnsEndPoint("your redis endpoint", 6379)
            };

            string connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;

            LokiConfigurationBuilder.Instance.SetServiceKey("SimpleTestClient")
                                    .SetPrimaryLockHandler(new RedisLokiLockHandler(redisEndPoints.ToArray()))
                                    .SetSecondaryLockHandler(new MSSQLLokiLockHandler(connectionString))
                                    .Build();
        }

        [TestMethod]
        public void TestClientLocking_WhenUseSharedResource_ShouldHaveNotUseSameItem()
        {
            // Arrange
            List<Task> clientTasks = new List<Task>
            {
                new Task(() =>
                {
                    for (int i = 0; i < Source.DummyItems.Count; i++)
                    {
                        _testClient.DebugWriteLine();
                    }
                }),

                new Task(() =>
                {
                    for (int i = 0; i < Source.DummyItems.Count; i++)
                    {
                        _testClient2.DebugWriteLine();
                    }
                })
            };



            //Act
            clientTasks.ForEach(x => x.Start());

            Task.WaitAll(clientTasks.ToArray());

            //Assert
            var dublicateItems = Source.ProcessedItems
                                .GroupBy(i => i)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key).ToList();

            Assert.AreEqual(0, dublicateItems.Count);
        }

        [TestMethod]
        public void TestClientLocking_WithUniqueKey_WhenUseSharedResource_ShouldHaveNotUseSameItem()
        {
            // Arrange
            List<Task> clientTasks = new List<Task>
            {
                new Task(() =>
                {
                    for (int i = 0; i < Source.DummyItems.Count; i++)
                    {
                        _testClient.DebugWriteLineWithUniqueKey();
                    }
                }),

                new Task(() =>
                {
                    for (int i = 0; i < Source.DummyItems.Count; i++)
                    {
                        _testClient2.DebugWriteLineWithUniqueKey();
                    }
                })
            };



            //Act
            clientTasks.ForEach(x => x.Start());

            Task.WaitAll(clientTasks.ToArray());

            //Assert
            var dublicateItems = Source.ProcessedItems
                                .GroupBy(i => i)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key).ToList();

            Assert.AreEqual(0, dublicateItems.Count);
        }
    }
}