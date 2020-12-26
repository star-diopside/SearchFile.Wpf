using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Services.FileFilters
{
    public interface IFilter
    {
        string Name { get; }
        IEnumerable<string> Patterns { get; }
    }
}
