using SearchFile.Wpf.Module.Models;
using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Services
{
    public interface IDeleteFileService
    {
        bool DeleteFiles(IEnumerable<Result> results, bool recycle);
    }
}
