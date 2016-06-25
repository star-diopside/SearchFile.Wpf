using SearchFile.Module.Models;
using System.Linq;
using System.Windows.Controls;

namespace SearchFile.Module.Views
{
    /// <summary>
    /// SearchFileView.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchFileView : UserControl
    {
        public SearchFileView()
        {
            InitializeComponent();
        }

        private void ResultsViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var result in e.RemovedItems.Cast<Result>())
            {
                result.IsSelected = false;
            }
            foreach (var result in e.AddedItems.Cast<Result>())
            {
                result.IsSelected = true;
            }
        }
    }
}
