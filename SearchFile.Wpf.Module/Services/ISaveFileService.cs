using SearchFile.Wpf.Module.Services.FileFilters;
using System.Collections.Generic;
using System.Linq;

namespace SearchFile.Wpf.Module.Services
{
    public interface ISaveFileService
    {
        string? SaveFile(IEnumerable<IFilter> filters);

        string? SaveFile(params IFilter[] filters) => SaveFile(filters.AsEnumerable());
    }
}
