using Microsoft.Practices.Unity;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using PropertyChanged;
using SearchFile.Messaging;
using SearchFile.Messaging.FileFilters;
using SearchFile.Models;
using System;

namespace SearchFile.ViewModels
{
    [ImplementPropertyChanged]
    public class SearchFileViewModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Condition Condition { get; set; }

        [Dependency]
        public Searcher Searcher { get; set; }

        public DelegateCommand ChooseFolderCommand { get; }
        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ClearResultsCommand { get; }
        public DelegateCommand SaveResultsCommand { get; }

        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        public SearchFileViewModel()
        {
            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = new DelegateCommand(() => this.Searcher.Clear());
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
        }

        private void ChooseFolder()
        {
            this.ChooseFolderRequest.Raise(new Notification()
            {
                Content = new ChooseFolderMessage()
                {
                    Path = this.Condition.TargetDirectory
                }
            }, n => this.Condition.TargetDirectory = ((ChooseFolderMessage)n.Content).Path);
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
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
                this.Searcher.SetError(ex);
            }
        }

        private void SaveResults()
        {
            try
            {
                this.SaveFileRequest.Raise(new Notification()
                {
                    Content = new SaveFileMessage()
                    {
                        Filters = { new CsvFileFilter(), new TextFileFilter(), new AllFileFilter() }
                    }
                }, n => this.Searcher.Save(((SaveFileMessage)n.Content).Path));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
            }
        }
    }
}
