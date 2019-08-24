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
using SearchFile.Wpf.Module.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SearchFile.Wpf.Module.ViewModels
{
    public class SearchFileViewModel : BindableBase, IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly CollectionViewSource resultsViewSource;
        private readonly ReactiveProperty<string> latestStatus = new ReactiveProperty<string>();
        private readonly ReactiveProperty<bool> isItemsSelected = new ReactiveProperty<bool>();

        public Models.Condition Condition { get; }
        private Searcher Searcher { get; }

        public ICollectionView ResultsView => this.resultsViewSource.View;
        public ReadOnlyReactiveProperty<bool> IsSearching => this.Searcher.IsSearching;
        public ReadOnlyReactiveProperty<bool> ExistsResults => this.Searcher.ExistsResults;
        public ReactiveProperty<bool> AutoAdjustColumnWidth { get; } = new ReactiveProperty<bool>(true);
        public ReadOnlyReactiveProperty<string> Status { get; }
        public ReactiveProperty<bool> RecyclesDeleteFiles { get; } = new ReactiveProperty<bool>(true);

        public ICommand ResultsViewSelectionChangedCommand { get; }
        public ICommand ChooseFolderCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearResultsCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand ReverseSelectionCommand { get; }
        public ICommand DeleteSelectionFileCommand { get; }
        public ICommand SaveResultsCommand { get; }
        public ICommand CopyResultsCommand { get; }
        public ICommand SortResultsCommand { get; }
        public ICommand ShowPropertyCommand { get; }

        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> AdjustColumnWidthRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> DeleteFileRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        public SearchFileViewModel(Models.Condition condition, Searcher searcher)
        {
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.Searcher = searcher ?? throw new ArgumentNullException(nameof(searcher));
            this.resultsViewSource = new CollectionViewSource() { Source = searcher.Results };

            searcher.SearchingDirectory.Subscribe(this.SearchingDirectoryChanged).AddTo(_disposable);

            ReactiveCommand ToReactiveCommand(Action execute, IObservable<bool> canExecute)
            {
                var command = canExecute.ToReactiveCommand();
                command.Subscribe(execute).AddTo(_disposable);
                return command;
            }

            this.ResultsViewSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(this.ResultsViewSelectionChanged);
            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = ToReactiveCommand(this.ClearResults, this.IsSearching.Inverse());
            this.SelectAllCommand = ToReactiveCommand(this.SelectAll, this.ExistsResults);
            this.ReverseSelectionCommand = ToReactiveCommand(this.ReverseSelection, this.ExistsResults);
            this.DeleteSelectionFileCommand = ToReactiveCommand(this.DeleteSelectionFile, this.ExistsResults);
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
            this.CopyResultsCommand = ToReactiveCommand(this.CopyResults, this.ExistsResults);
            this.SortResultsCommand = new DelegateCommand<string>(this.SortResults);
            this.ShowPropertyCommand = ToReactiveCommand(this.ShowProperty, this.isItemsSelected);
            this.Status = this.latestStatus.ToReadOnlyReactiveProperty();
        }

        public void Dispose()
        {
            _disposable.Dispose();
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

        private void ResultsViewSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (var result in e.RemovedItems.Cast<Result>())
            {
                result.IsSelected.Value = false;
            }
            foreach (var result in e.AddedItems.Cast<Result>())
            {
                result.IsSelected.Value = true;
            }

            this.isItemsSelected.Value = this.Searcher.Results.Any(result => result.IsSelected.Value);
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
                    if (this.AutoAdjustColumnWidth.Value)
                    {
                        this.AdjustColumnWidthRequest.Raise(new Notification());
                    }
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

        private void ShowProperty()
        {
            var files = from result in this.Searcher.Results
                        where result.IsSelected.Value
                        select result.FilePath;

            foreach (var file in files)
            {
                FileOperate.ShowPropertyDialog(file);
            }
        }
    }
}
