using GalaSoft.MvvmLight.Messaging;
using SearchFile.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Trigger
{
    public class SaveFileTrigger : TriggerBase<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<SaveFileMessage>(AssociatedObject, InvokeActions);
        }

        protected override void OnDetaching()
        {
            Messenger.Default.Unregister<SaveFileMessage>(AssociatedObject);
            base.OnDetaching();
        }
    }
}
