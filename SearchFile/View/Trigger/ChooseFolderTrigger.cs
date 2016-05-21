using GalaSoft.MvvmLight.Messaging;
using SearchFile.Messaging;
using System.Windows;
using System.Windows.Interactivity;

namespace SearchFile.View.Trigger
{
    public class ChooseFolderTrigger : TriggerBase<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<ChooseFolderMessage>(AssociatedObject, InvokeActions);
        }

        protected override void OnDetaching()
        {
            Messenger.Default.Unregister<ChooseFolderMessage>(AssociatedObject);
            base.OnDetaching();
        }
    }
}
