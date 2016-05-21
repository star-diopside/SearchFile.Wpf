using Microsoft.WindowsAPICodePack.Dialogs;
using SearchFile.Messaging;
using SearchFile.Properties;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace SearchFile.View.Action
{
    public class ExceptionAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var em = parameter as ExceptionMessage;
            if (em != null)
            {
                using (var dialog = new TaskDialog())
                {
                    dialog.OwnerWindowHandle = new WindowInteropHelper(Window.GetWindow(AssociatedObject)).Handle;
                    dialog.Caption = Resources.ErrorDialogCaption;
                    dialog.InstructionText = Resources.OccurExceptionMessage;
                    dialog.Text = em.Exception.Message;
                    dialog.Icon = TaskDialogStandardIcon.Error;
                    dialog.StandardButtons = TaskDialogStandardButtons.Ok;
                    dialog.Show();
                }
            }
        }
    }
}
