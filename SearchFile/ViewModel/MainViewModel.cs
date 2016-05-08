using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using SearchFile.Model;

namespace SearchFile.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBase
    {
        public Condition Condition { get; } = new Condition();
        public Searcher Searcher { get; } = new Searcher();

        public RelayCommand SearchCommand { get; }

        public MainViewModel()
        {
            this.SearchCommand = new RelayCommand(Search);
        }

        private async void Search()
        {
            await this.Searcher.Search(this.Condition);
        }
    }
}
