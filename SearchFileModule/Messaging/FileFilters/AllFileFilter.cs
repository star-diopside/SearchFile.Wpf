using System.Collections.Generic;

namespace SearchFile.Messaging.FileFilters
{
    public class AllFileFilter : IFilter
    {
        public string Name { get; } = "すべてのファイル";
        public IEnumerable<string> Patterns { get; } = new[] { "*.*" };
    }
}
