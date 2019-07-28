using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Interactivity.InteractionRequest;
using SearchFile.Wpf.Module.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class ChooseFolderAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (!(parameter is InteractionRequestedEventArgs args && args.Context?.Content is ChooseFolderMessage message))
            {
                return;
            }

            using var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = message.Path;

            if (dialog.ShowDialog(Window.GetWindow(this.AssociatedObject)) == CommonFileDialogResult.Ok)
            {
                message.Path = dialog.FileName;
                args.Callback();
            }
        }
    }
}
