using System.Collections.Generic;

namespace SearchFile.Module.Messaging.FileFilters
{
    public interface IFilter
    {
        string Name { get; }
        IEnumerable<string> Patterns { get; }
    }
}
