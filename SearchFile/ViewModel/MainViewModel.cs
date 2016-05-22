using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;
using PropertyChanged;
using SearchFile.Messaging;
using SearchFile.Model;
using System;

namespace SearchFile.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Condition Condition { get; } = new Condition();
        public Searcher Searcher { get; } = new Searcher();

        public RelayCommand ChooseFolderCommand { get; }
        public RelayCommand SearchCommand { get; }
        public RelayCommand ClearResultsCommand { get; }
        public RelayCommand SaveResultsCommand { get; }

        public MainViewModel()
        {
            this.ChooseFolderCommand = new RelayCommand(ChooseFolder);
            this.SearchCommand = new RelayCommand(Search);
            this.ClearResultsCommand = new RelayCommand(this.Searcher.Clear);
            this.SaveResultsCommand = new RelayCommand(this.SaveResults);
        }

        private void ChooseFolder()
        {
            MessengerInstance.Send(new ChooseFolderMessage()
            {
                Path = this.Condition.TargetDirectory,
                Callback = path => this.Condition.TargetDirectory = path
            });
        }

        private async void Search()
        {
            try
            {
                if (this.Searcher.IsSearching)
                {
                    this.Searcher.Cancel();
                }
                else
                {
                    await this.Searcher.Search(this.Condition);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                MessengerInstance.Send(new ExceptionMessage(ex));
                this.Searcher.SetError(ex);
            }
        }

        private void SaveResults()
        {
            MessengerInstance.Send(new SaveFileMessage()
            {
                Filters = { new SaveFileMessage.Filter() { Name = "CSVファイル", Patterns = { "*.csv" } },
                    new SaveFileMessage.Filter() { Name = "テキストファイル", Patterns = { "*.txt" } },
                    new SaveFileMessage.Filter() { Name = "すべてのファイル", Patterns = { "*.*" } } },
                Callback = path => this.Searcher.Save(path)
            });
        }
    }
}
