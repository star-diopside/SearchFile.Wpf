using Prism.Interactivity.InteractionRequest;
using SearchFile.Module.Messaging;
using SearchFile.Module.Shell;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Module.Views.Action
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

            message.IsSuccessful = FileOperate.DeleteFiles(Window.GetWindow(this.AssociatedObject),
                message.Results.Select(result => result.FilePath), message.Recycle);

            args.Callback();
        }
    }
}
