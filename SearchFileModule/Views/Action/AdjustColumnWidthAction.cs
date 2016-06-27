using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SearchFile.Module.Views.Action
{
    public class AdjustColumnWidthAction : TriggerAction<GridView>
    {
        protected override void Invoke(object parameter)
        {
            foreach (var column in this.AssociatedObject.Columns)
            {
                column.Width = 0;
                column.Width = double.NaN;
            }
        }
    }
}
