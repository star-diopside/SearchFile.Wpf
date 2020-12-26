using Microsoft.WindowsAPICodePack.Dialogs;
using SearchFile.Wpf.Module.Properties;
using System;
using System.Windows;
using System.Windows.Interop;

namespace SearchFile.Wpf.Module.Services
{
    public class ExceptionService : IExceptionService
    {
        public void ShowDialog(Exception exception)
        {
            using var dialog = new TaskDialog
            {
                OwnerWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle,
                Caption = Resources.ErrorDialogCaption,
                InstructionText = Resources.OccurExceptionMessage,
                Text = exception.Message,
                Icon = TaskDialogStandardIcon.Error,
                StandardButtons = TaskDialogStandardButtons.Ok
            };

            dialog.Show();
        }
    }
}
