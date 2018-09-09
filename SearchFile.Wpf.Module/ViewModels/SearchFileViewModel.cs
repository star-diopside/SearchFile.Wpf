using Microsoft.Practices.Unity;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SearchFile.Wpf.Module.Messaging;
using SearchFile.Wpf.Module.Messaging.FileFilters;
using SearchFile.Wpf.Module.Models;
using SearchFile.Wpf.Module.Properties;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.Wpf.Module.ViewModels
{
    public class SearchFileViewModel : BindableBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly CollectionViewSource resultsViewSource;
        private readonly ReactiveProperty<string> latestStatus = new ReactiveProperty<string>();

        public SearchFile.Wpf.Module.Models.Condition Condition { get; }
        private Searcher Searcher { get; }

        public ICollectionView ResultsView => this.resultsViewSource.View;
        public ReadOnlyReactiveProperty<bool> IsSearching => this.Searcher.IsSearching;
        public ReadOnlyReactiveProperty<bool> ExistsResults => this.Searcher.ExistsResults;
        public ReadOnlyReactiveProperty<string> Status { get; }
        public ReactiveProperty<bool> RecyclesDeleteFiles { get; set; } = new ReactiveProperty<bool>(true);

        public DelegateCommand ChooseFolderCommand { get; }
        public DelegateCommand SearchCommand { get; }
        public ReactiveCommand ClearResultsCommand { get; }
        public ReactiveCommand SelectAllCommand { get; }
        public ReactiveCommand ReverseSelectionCommand { get; }
        public ReactiveCommand DeleteSelectionFileCommand { get; }
        public DelegateCommand SaveResultsCommand { get; }
        public ReactiveCommand CopyResultsCommand { get; }
        public DelegateCommand<string> SortResultsCommand { get; }

        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> AdjustColumnWidthRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> DeleteFileRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        [InjectionConstructor]
        public SearchFileViewModel(SearchFile.Wpf.Module.Models.Condition condition, Searcher searcher)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            this.Condition = condition;
            this.Searcher = searcher;
            this.resultsViewSource = new CollectionViewSource() { Source = searcher.Results };

            searcher.SearchingDirectory.Subscribe(this.SearchingDirectoryChanged);

            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = this.IsSearching.Inverse().ToReactiveCommand();
            this.ClearResultsCommand.Subscribe(this.ClearResults);
            this.SelectAllCommand = this.ExistsResults.ToReactiveCommand();
            this.SelectAllCommand.Subscribe(this.SelectAll);
            this.ReverseSelectionCommand = this.ExistsResults.ToReactiveCommand();
            this.ReverseSelectionCommand.Subscribe(this.ReverseSelection);
            this.DeleteSelectionFileCommand = this.ExistsResults.ToReactiveCommand();
            this.DeleteSelectionFileCommand.Subscribe(this.DeleteSelectionFile);
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
            this.CopyResultsCommand = this.ExistsResults.ToReactiveCommand();
            this.CopyResultsCommand.Subscribe(this.CopyResults);
            this.SortResultsCommand = new DelegateCommand<string>(this.SortResults);

            this.Status = this.latestStatus.ToReadOnlyReactiveProperty();
        }

        private void SearchingDirectoryChanged(string directory)
        {
            if (directory == null)
            {
                this.latestStatus.Value = string.Format(Resources.SearchingResultMessage, this.Searcher.Results.Count);
            }
            else
            {
                this.latestStatus.Value = string.Format(Resources.SearchingDirectoryMessage, directory);
            }
        }

        private void ChooseFolder()
        {
            this.ChooseFolderRequest.Raise(new Notification()
            {
                Content = new ChooseFolderMessage()
                {
                    Path = this.Condition.TargetDirectory.Value
                }
            }, n => this.Condition.TargetDirectory.Value = ((ChooseFolderMessage)n.Content).Path);
        }

        private async void Search()
        {
            try
            {
                if (this.Searcher.IsSearching.Value)
                {
                    this.Searcher.Cancel();
                }
                else
                {
                    await this.Searcher.SearchAsync(this.Condition);
                    this.AdjustColumnWidthRequest.Raise(new Notification());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
                this.latestStatus.Value = Resources.SearchingErrorMessage;
            }
        }

        private void ClearResults()
        {
            this.Searcher.Clear();
            this.resultsViewSource.SortDescriptions.Clear();
            this.latestStatus.Value = Resources.ClearResultsMessage;
        }

        private void SelectAll()
        {
            foreach (var result in this.Searcher.Results)
            {
                result.IsSelected.Value = true;
            }
        }

        private void ReverseSelection()
        {
            foreach (var result in this.Searcher.Results)
            {
                result.IsSelected.Value = !result.IsSelected.Value;
            }
        }

        private void DeleteSelectionFile()
        {
            this.DeleteFileRequest.Raise(new Notification()
            {
                Content = new DeleteFileMessage()
                {
                    Results = this.Searcher.Results.Where(result => result.IsSelected.Value).ToArray(),
                    Recycle = this.RecyclesDeleteFiles.Value
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
                    this.latestStatus.Value = string.Format(Resources.FileDeleteMessage, message.Results.Count());
                }
                else
                {
                    this.latestStatus.Value = Resources.FileDeleteCancelMessage;
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
            this.latestStatus.Value = string.Format(Resources.CopyFileNameMessage, this.Searcher.Results.Count);
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
