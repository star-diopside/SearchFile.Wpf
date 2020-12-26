using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace SearchFile.Wpf.Module.Views.Action
{
    public class AdjustColumnWidthAction : TriggerAction<GridView>
    {
        public static readonly DependencyProperty AutoAdjustColumnWidthProperty = DependencyProperty.Register(
            nameof(AutoAdjustColumnWidth),
            typeof(bool),
            typeof(AdjustColumnWidthAction));

        public bool AutoAdjustColumnWidth
        {
            get => (bool)GetValue(AutoAdjustColumnWidthProperty);
            set => SetValue(AutoAdjustColumnWidthProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            if (AutoAdjustColumnWidth &&
                parameter is DependencyPropertyChangedEventArgs e &&
                e.OldValue is true &&
                e.NewValue is false)
            {
                foreach (var column in AssociatedObject.Columns)
                {
                    column.Width = 0;
                    column.Width = double.NaN;
                }
            }
        }
    }
}
