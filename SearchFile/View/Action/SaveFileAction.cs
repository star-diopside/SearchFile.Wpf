using Microsoft.Win32;
using SearchFile.Messaging;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Action
{
    public class SaveFileAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var sfm = parameter as SaveFileMessage;
            if (sfm != null)
            {
                var dialog = new SaveFileDialog();

                dialog.FileName = sfm.Path;
                dialog.Filter = string.Join("|", from filter in sfm.Filters
                                                 let pattern = string.Join(";", filter.Patterns)
                                                 select $"{filter.Name} ({pattern})|{pattern}");

                if (dialog.ShowDialog(Window.GetWindow(AssociatedObject)) == true)
                {
                    sfm.Callback(dialog.FileName);
                }
            }
        }
    }
}
