using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;
using SearchFile.Messaging;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Views.Action
{
    public class SaveFileAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var sfm = args?.Context?.Content as SaveFileMessage;
            if (sfm == null)
            {
                return;
            }

            var dialog = new SaveFileDialog()
            {
                FileName = sfm.Path,
                Filter = string.Join("|", from filter in sfm.Filters
                                          let pattern = string.Join(";", filter.Patterns)
                                          select $"{filter.Name} ({pattern})|{pattern}")
            };

            if (dialog.ShowDialog(Window.GetWindow(this.AssociatedObject)) == true)
            {
                sfm.Path = dialog.FileName;
                args.Callback();
            }
        }
    }
}
