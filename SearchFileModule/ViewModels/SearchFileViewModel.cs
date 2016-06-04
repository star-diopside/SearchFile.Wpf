using Microsoft.Practices.Unity;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using PropertyChanged;
using SearchFile.Messaging;
using SearchFile.Messaging.FileFilters;
using SearchFile.Models;
using SearchFileModule.Properties;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.ViewModels
{
    [ImplementPropertyChanged]
    public class SearchFileViewModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Searcher searcher;
        private CollectionViewSource resultsViewSource;

        [Dependency]
        public SearchFile.Models.Condition Condition { get; set; }

        [Dependency]
        public Searcher Searcher
        {
            get
            {
                return this.searcher;
            }
            set
            {
                if (this.searcher != null)
                {
                    PropertyChangedEventManager.RemoveHandler(this.searcher, this.SearcherStatusChanged, nameof(Searcher.Status));
                }

                this.searcher = value;
                this.resultsViewSource = new CollectionViewSource() { Source = this.searcher.Results };

                if (searcher != null)
                {
                    PropertyChangedEventManager.AddHandler(this.searcher, this.SearcherStatusChanged, nameof(Searcher.Status));
                }
            }
        }

        public ICollectionView ResultsView => this.resultsViewSource.View;
        public string Status { get; private set; }

        public DelegateCommand ChooseFolderCommand { get; }
        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ClearResultsCommand { get; }
        public DelegateCommand SaveResultsCommand { get; }
        public DelegateCommand CopyResultsCommand { get; }
        public DelegateCommand<string> SortResultsCommand { get; }

        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        public SearchFileViewModel()
        {
            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = new DelegateCommand(() => this.Searcher.Clear());
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
            this.CopyResultsCommand = new DelegateCommand(this.CopyResults);
            this.SortResultsCommand = new DelegateCommand<string>(this.SortResults);
        }

        private void SearcherStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Status = ((Searcher)sender).Status;
        }

        public void SetError(Exception ex)
        {
            this.Status = Resources.SearchingErrorMessage;
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
                this.SetError(ex);
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

        private void CopyResults()
        {
            var sb = new StringBuilder();

            foreach (var result in this.Searcher.Results)
            {
                sb.AppendLine(result.FilePath);
            }

            Clipboard.SetText(sb.ToString());
            this.Status = string.Format(Resources.CopyFileNameMessage, this.Searcher.Results.Count);
        }

        private void SortResults(string propertyName)
        {
            var direction = (from sd in this.resultsViewSource.SortDescriptions
                             where sd.PropertyName == propertyName
                             select sd.Direction).DefaultIfEmpty(ListSortDirection.Descending).First();

            this.resultsViewSource.SortDescriptions.Clear();
            this.resultsViewSource.SortDescriptions.Add(new SortDescription(propertyName,
                direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending));
        }
    }
}
