using SearchFile.Wpf.Module.Models;
using SearchFile.Wpf.Module.Shell;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SearchFile.Wpf.Module.Services
{
    public class DeleteFileService : IDeleteFileService
    {
        public bool DeleteFiles(IEnumerable<Result> results, bool recycle)
        {
            return FileOperate.DeleteFiles(Application.Current.MainWindow,
                                           results.Select(result => result.FilePath),
                                           recycle);
        }
    }
}
