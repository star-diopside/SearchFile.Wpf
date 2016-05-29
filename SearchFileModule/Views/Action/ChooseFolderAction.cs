using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Interactivity.InteractionRequest;
using SearchFile.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Views.Action
{
    public class ChooseFolderAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var cfm = args?.Context?.Content as ChooseFolderMessage;
            if (cfm == null)
            {
                return;
            }

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = cfm.Path;

                if (dialog.ShowDialog(Window.GetWindow(this.AssociatedObject)) == CommonFileDialogResult.Ok)
                {
                    cfm.Path = dialog.FileName;
                    args.Callback();
                }
            }
        }
    }
}
