using GalaSoft.MvvmLight.Messaging;
using SearchFile.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Trigger
{
    public class ExceptionTrigger : TriggerBase<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<ExceptionMessage>(AssociatedObject, InvokeActions);
        }

        protected override void OnDetaching()
        {
            Messenger.Default.Unregister<ExceptionMessage>(AssociatedObject);
            base.OnDetaching();
        }
    }
}
