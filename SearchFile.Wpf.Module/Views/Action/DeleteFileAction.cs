using Prism.Interactivity.InteractionRequest;
using SearchFile.Wpf.Module.Messaging;
using SearchFile.Wpf.Module.Shell;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class DeleteFileAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (!(parameter is InteractionRequestedEventArgs args && args.Context?.Content is DeleteFileMessage message))
            {
                return;
            }

            message.IsSuccessful = FileOperate.DeleteFiles(Window.GetWindow(this.AssociatedObject),
                message.Results.Select(result => result.FilePath), message.Recycle);

            args.Callback();
        }
    }
}
