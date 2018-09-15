using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;
using SearchFile.Wpf.Module.Messaging;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class SaveFileAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var message = args?.Context?.Content as SaveFileMessage;
            if (message == null)
            {
                return;
            }

            var dialog = new SaveFileDialog()
            {
                FileName = message.Path,
                Filter = string.Join("|", from filter in message.Filters
                                          let pattern = string.Join(";", filter.Patterns)
                                          select $"{filter.Name} ({pattern})|{pattern}")
            };

            if (dialog.ShowDialog(Window.GetWindow(this.AssociatedObject)) == true)
            {
                message.Path = dialog.FileName;
                args.Callback();
            }
        }
    }
}
