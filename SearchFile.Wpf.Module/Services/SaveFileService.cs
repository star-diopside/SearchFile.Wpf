using Microsoft.Win32;
using SearchFile.Wpf.Module.Services.FileFilters;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SearchFile.Wpf.Module.Services
{
    public class SaveFileService : ISaveFileService
    {
        public string? SaveFile(IEnumerable<IFilter> filters)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = string.Join('|', from filter in filters
                                          let pattern = string.Join(';', filter.Patterns)
                                          select $"{filter.Name} ({pattern})|{pattern}")
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
