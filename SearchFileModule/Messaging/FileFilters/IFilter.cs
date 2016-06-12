using System.Collections.Generic;

namespace SearchFile.Messaging.FileFilters
{
    public interface IFilter
    {
        string Name { get; }
        IEnumerable<string> Patterns { get; }
    }
}
