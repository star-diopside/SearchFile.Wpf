using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class WindowCloseAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            Window.GetWindow(AssociatedObject).Close();
        }
    }
}
