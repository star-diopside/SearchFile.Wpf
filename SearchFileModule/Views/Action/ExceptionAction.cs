using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Interactivity.InteractionRequest;
using SearchFileModule.Properties;
using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace SearchFile.Views.Action
{
    public class ExceptionAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var ex = args?.Context?.Content as Exception;
            if (ex == null)
            {
                return;
            }

            using (var dialog = new TaskDialog())
            {
                dialog.OwnerWindowHandle = new WindowInteropHelper(Window.GetWindow(this.AssociatedObject)).Handle;
                dialog.Caption = Resources.ErrorDialogCaption;
                dialog.InstructionText = Resources.OccurExceptionMessage;
                dialog.Text = ex.Message;
                dialog.Icon = TaskDialogStandardIcon.Error;
                dialog.StandardButtons = TaskDialogStandardButtons.Ok;
                dialog.Show();
            }
        }
    }
}
