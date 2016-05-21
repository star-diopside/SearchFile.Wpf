using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Action
{
    public class WindowCloseAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            Window.GetWindow(AssociatedObject).Close();
        }
    }
}
