using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Interactivity.InteractionRequest;
using SearchFile.Wpf.Module.Properties;
using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class ExceptionAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (!(parameter is InteractionRequestedEventArgs args && args.Context?.Content is Exception ex))
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
