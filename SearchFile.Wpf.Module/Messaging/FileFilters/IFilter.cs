using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Messaging.FileFilters
{
    public interface IFilter
    {
        string Name { get; }
        IEnumerable<string> Patterns { get; }
    }
}
