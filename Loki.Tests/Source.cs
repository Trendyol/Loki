using System.Collections.Generic;

namespace Loki.Tests
{
    public class Source
    {
        public static readonly List<DummyItem> DummyItems = new List<DummyItem>();
        public static readonly List<DummyItem> ProcessedItems = new List<DummyItem>();
    }

    public class DummyItem
    {
        public int Id { get; set; }
        public bool IsProcessed { get; set; }
    }
}