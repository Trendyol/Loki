using System.Linq;
using System.Threading;

namespace Loki.Tests
{
    public class TestClient
    {
        public void DebugWriteLine()
        {
            Locking.Instance.ExecuteWithinLock(() =>
            {
                var item = Source.DummyItems.FirstOrDefault(queryable => queryable.IsProcessed == false);

                if (item != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Id: {item.Id} Is Processed: {item.IsProcessed} Thread {Thread.CurrentThread.ManagedThreadId}");

                    item.IsProcessed = true;

                    Source.ProcessedItems.Add(item);
                }
            }, expiryFromSeconds: 5);
        }
    }
}