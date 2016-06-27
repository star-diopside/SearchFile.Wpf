using Microsoft.Practices.Unity;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using PropertyChanged;
using SearchFile.Module.Messaging;
using SearchFile.Module.Messaging.FileFilters;
using SearchFile.Module.Models;
using SearchFile.Module.Properties;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.Module.ViewModels
{
    [ImplementPropertyChanged]
    public class SearchFileViewModel : BindableBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly CollectionViewSource resultsViewSource;

        [Dependency]
        public SearchFile.Module.Models.Condition Condition { get; set; }

        private Searcher Searcher { get; }

        public ICollectionView ResultsView => this.resultsViewSource.View;
        public bool IsSearching => this.Searcher.IsSearching;
        public bool ExistsResults => this.Searcher.ExistsResults;
        public string Status { get; private set; }
        public bool RecyclesDeleteFiles { get; set; } = true;

        public DelegateCommand ChooseFolderCommand { get; }
        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ClearResultsCommand { get; }
        public DelegateCommand SelectAllCommand { get; }
        public DelegateCommand ReverseSelectionCommand { get; }
        public DelegateCommand DeleteSelectionFileCommand { get; }
        public DelegateCommand SaveResultsCommand { get; }
        public DelegateCommand CopyResultsCommand { get; }
        public DelegateCommand<string> SortResultsCommand { get; }

        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> AdjustColumnWidthRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> DeleteFileRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        public SearchFileViewModel()
        {
            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = new DelegateCommand(this.ClearResults);
            this.SelectAllCommand = new DelegateCommand(this.SelectAll);
            this.ReverseSelectionCommand = new DelegateCommand(this.ReverseSelection);
            this.DeleteSelectionFileCommand = new DelegateCommand(this.DeleteSelectionFile);
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
            this.CopyResultsCommand = new DelegateCommand(this.CopyResults);
            this.SortResultsCommand = new DelegateCommand<string>(this.SortResults);
        }

        [InjectionConstructor]
        public SearchFileViewModel(Searcher searcher) : this()
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            this.Searcher = searcher;
            this.resultsViewSource = new CollectionViewSource() { Source = searcher.Results };

            PropertyChangedEventManager.AddHandler(searcher, (s, e) => this.OnPropertyChanged(nameof(IsSearching)), nameof(searcher.IsSearching));
            PropertyChangedEventManager.AddHandler(searcher, (s, e) => this.OnPropertyChanged(nameof(ExistsResults)), nameof(searcher.ExistsResults));
            PropertyChangedEventManager.AddHandler(searcher, this.SearchingDirectoryChanged, nameof(searcher.SearchingDirectory));
        }

        private void SearchingDirectoryChanged(object sender, PropertyChangedEventArgs e)
        {
            var searcher = (Searcher)sender;
            var directory = searcher.SearchingDirectory;

            if (directory == null)
            {
                this.Status = string.Format(Resources.SearchingResultMessage, searcher.Results.Count);
            }
            else
            {
                this.Status = string.Format(Resources.SearchingDirectoryMessage, directory);
            }
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
                    this.AdjustColumnWidthRequest.Raise(new Notification());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
                this.Status = Resources.SearchingErrorMessage;
            }
        }

        private void ClearResults()
        {
            this.Searcher.Clear();
            this.resultsViewSource.SortDescriptions.Clear();
            this.Status = Resources.ClearResultsMessage;
        }

        private void SelectAll()
        {
            foreach (var result in this.Searcher.Results)
            {
                result.IsSelected = true;
            }
        }

        private void ReverseSelection()
        {
            foreach (var result in this.Searcher.Results)
            {
                result.IsSelected = !result.IsSelected;
            }
        }

        private void DeleteSelectionFile()
        {
            this.DeleteFileRequest.Raise(new Notification()
            {
                Content = new DeleteFileMessage()
                {
                    Results = this.Searcher.Results.Where(result => result.IsSelected).ToArray(),
                    Recycle = this.RecyclesDeleteFiles
                }
            }, n =>
            {
                var message = (DeleteFileMessage)n.Content;

                if (message.IsSuccessful)
                {
                    foreach (var result in message.Results)
                    {
                        this.Searcher.Results.Remove(result);
                    }
                    this.Status = string.Format(Resources.FileDeleteMessage, message.Results.Count());
                }
                else
                {
                    this.Status = Resources.FileDeleteCancelMessage;
                }
            });
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
