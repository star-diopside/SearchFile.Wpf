using Microsoft.WindowsAPICodePack.Dialogs;
using SearchFile.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Action
{
    public class ChooseFolderAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var cfm = parameter as ChooseFolderMessage;
            if (cfm != null)
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.InitialDirectory = cfm.Path;
                    if (dialog.ShowDialog(Window.GetWindow(AssociatedObject)) == CommonFileDialogResult.Ok)
                    {
                        cfm.Callback(dialog.FileName);
                    }
                }
            }
        }
    }
}
