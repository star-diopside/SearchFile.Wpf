using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.Views.Action
{
    public class WindowCloseAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            Window.GetWindow(this.AssociatedObject).Close();
        }
    }
}
