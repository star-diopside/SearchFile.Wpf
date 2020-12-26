using SearchFile.Wpf.Module.Services.FileFilters;
using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Services
{
    public interface ISaveFileService
    {
        string? SaveFile(ICollection<IFilter> filters);
    }
}
