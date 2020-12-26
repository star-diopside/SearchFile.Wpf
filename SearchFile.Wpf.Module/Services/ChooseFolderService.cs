using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace SearchFile.Wpf.Module.Services
{
    public class ChooseFolderService : IChooseFolderService
    {
        public string? ShowDialog(string? initialDirectory)
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
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
