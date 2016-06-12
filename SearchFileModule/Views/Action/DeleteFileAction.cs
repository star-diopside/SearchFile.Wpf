using Prism.Interactivity.InteractionRequest;
using SearchFile.Messaging;
using SearchFile.WindowsShell;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Views.Action
{
    public class DeleteFileAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var message = args?.Context?.Content as DeleteFileMessage;
            if (message == null)
            {
                return;
            }

            FileOperate.DeleteFiles(Window.GetWindow(this.AssociatedObject),
                message.Results.Select(result => result.FilePath), message.Recycle);

            args.Callback();
        }
    }
}
