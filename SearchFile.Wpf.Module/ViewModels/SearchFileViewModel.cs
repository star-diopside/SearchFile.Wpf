using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SearchFile.Wpf.Module.Models;
using SearchFile.Wpf.Module.Properties;
using SearchFile.Wpf.Module.Services;
using SearchFile.Wpf.Module.Services.FileFilters;
using SearchFile.Wpf.Module.Shell;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SearchFile.Wpf.Module.ViewModels
{
    public class SearchFileViewModel : BindableBase, IDisposable
    {
        private readonly ILogger<SearchFileViewModel> _logger;
        private readonly IExceptionService _exceptionService;
        private readonly IChooseFolderService _chooseFolderService;
        private readonly IDeleteFileService _deleteFileService;
        private readonly ISaveFileService _saveFileService;
        private readonly ICondition _condition;
        private readonly ISearcher _searcher;

        private readonly CompositeDisposable _disposable = new();
        private readonly CollectionViewSource _resultsViewSource;
        private readonly ReactivePropertySlim<string?> _status = new();
        private readonly ReactivePropertySlim<bool> _isItemsSelected = new();
        private readonly ReactivePropertySlim<CancellationTokenSource?> _cancellationTokenSource = new();

        public ReactiveProperty<string?> TargetDirectory { get; }
        public ReactivePropertySlim<string?> FileName => _condition.FileName;
        public ReactivePropertySlim<FileNameMatchType> MatchType => _condition.MatchType;
        public ICollectionView ResultsView => _resultsViewSource.View;
        public ReadOnlyReactivePropertySlim<bool> IsSearching { get; }
        public ReadOnlyReactivePropertySlim<bool> ExistsResults { get; }
        public ReactivePropertySlim<bool> AutoAdjustColumnWidth { get; } = new(true);
        public ReadOnlyReactivePropertySlim<string?> Status { get; }
        public ReactivePropertySlim<bool> RecyclesDeleteFiles { get; } = new(true);

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

        public SearchFileViewModel(ILogger<SearchFileViewModel> logger,
                                   IExceptionService exceptionService,
                                   IChooseFolderService chooseFolderService,
                                   IDeleteFileService deleteFileService,
                                   ISaveFileService saveFileService,
                                   ICondition condition,
                                   ISearcher searcher)
        {
            _logger = logger;
            _exceptionService = exceptionService;
            _chooseFolderService = chooseFolderService;
            _deleteFileService = deleteFileService;
            _saveFileService = saveFileService;
            _condition = condition;
            _searcher = searcher;

            _resultsViewSource = new() { Source = searcher.Results };
            _searcher.SearchingDirectory.Subscribe(SearchingDirectoryChanged).AddTo(_disposable);

            TargetDirectory = _condition.TargetDirectory.ToReactiveProperty()
                .SetValidateNotifyError(dir => Directory.Exists(dir) ? null : Resources.DirectoryNotFoundMessage);
            IsSearching = _cancellationTokenSource.Select(s => s is not null).ToReadOnlyReactivePropertySlim();
            ExistsResults = _searcher.Results.CollectionChangedAsObservable().Select(_ => _searcher.Results.Any()).ToReadOnlyReactivePropertySlim();
            Status = _status.ToReadOnlyReactivePropertySlim();

            ResultsViewSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(ResultsViewSelectionChanged);
            ChooseFolderCommand = new DelegateCommand(ChooseFolder);
            SearchCommand = TargetDirectory.ObserveHasErrors.Inverse().CombineLatest(IsSearching, (x, y) => x || y)
                .ToReactiveCommand().WithSubscribe(Search).AddTo(_disposable);
            ClearResultsCommand = IsSearching.Inverse().ToReactiveCommand().WithSubscribe(ClearResults).AddTo(_disposable);
            SelectAllCommand = ExistsResults.ToReactiveCommand().WithSubscribe(SelectAll).AddTo(_disposable);
            ReverseSelectionCommand = ExistsResults.ToReactiveCommand().WithSubscribe(ReverseSelection).AddTo(_disposable);
            DeleteSelectionFileCommand = ExistsResults.ToReactiveCommand().WithSubscribe(DeleteSelectionFile).AddTo(_disposable);
            SaveResultsCommand = new DelegateCommand(SaveResults);
            CopyResultsCommand = ExistsResults.ToReactiveCommand().WithSubscribe(CopyResults).AddTo(_disposable);
            SortResultsCommand = new DelegateCommand<string>(SortResults);
            ShowPropertyCommand = _isItemsSelected.ToReactiveCommand().WithSubscribe(ShowProperty).AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void SearchingDirectoryChanged(string? directory)
        {
            if (directory is null)
            {
                _status.Value = string.Format(Resources.SearchingResultMessage, _searcher.Results.Count);
            }
            else
            {
                _status.Value = string.Format(Resources.SearchingDirectoryMessage, directory);
            }
        }

        private void ResultsViewSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (Result result in e.RemovedItems)
            {
                result.IsSelected.Value = false;
            }

            foreach (Result result in e.AddedItems)
            {
                result.IsSelected.Value = true;
            }

            _isItemsSelected.Value = _searcher.Results.Any(result => result.IsSelected.Value);
        }

        private void ChooseFolder()
        {
            string? path = _chooseFolderService.ShowDialog(_condition.TargetDirectory.Value);

            if (path is not null)
            {
                _condition.TargetDirectory.Value = path;
            }
        }

        private async void Search()
        {
            if (_cancellationTokenSource.Value is null)
            {
                using var cancellation = (_cancellationTokenSource.Value = new());

                try
                {
                    await _searcher.SearchAsync(_condition, cancellation.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    _exceptionService.ShowDialog(ex);
                    _status.Value = Resources.SearchingErrorMessage;
                }
                finally
                {
                    _cancellationTokenSource.Value = null;
                }
            }
            else
            {
                _cancellationTokenSource.Value.Cancel();
            }
        }

        private void ClearResults()
        {
            _searcher.Clear();
            _resultsViewSource.SortDescriptions.Clear();
            _status.Value = Resources.ClearResultsMessage;
        }

        private void SelectAll()
        {
            foreach (var result in _searcher.Results)
            {
                result.IsSelected.Value = true;
            }
        }

        private void ReverseSelection()
        {
            foreach (var result in _searcher.Results)
            {
                result.IsSelected.Value = !result.IsSelected.Value;
            }
        }

        private void DeleteSelectionFile()
        {
            var results = _searcher.Results.Where(result => result.IsSelected.Value).ToArray();

            if (_deleteFileService.DeleteFiles(results, RecyclesDeleteFiles.Value))
            {
                _searcher.Remove(results);
                _status.Value = string.Format(Resources.FileDeleteMessage, results.Count());
            }
            else
            {
                _status.Value = Resources.FileDeleteCancelMessage;
            }
        }

        private void SaveResults()
        {
            try
            {
                string? path = _saveFileService.SaveFile(new CsvFileFilter(),
                                                         new TextFileFilter(),
                                                         new AllFileFilter());

                if (path is not null)
                {
                    _searcher.Save(path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _exceptionService.ShowDialog(ex);
            }
        }

        private void CopyResults()
        {
            var sb = new StringBuilder();

            foreach (var result in _searcher.Results)
            {
                sb.AppendLine(result.FilePath);
            }

            Clipboard.SetText(sb.ToString());
            _status.Value = string.Format(Resources.CopyFileNameMessage, _searcher.Results.Count);
        }

        private void SortResults(string propertyName)
        {
            var direction = (from sd in _resultsViewSource.SortDescriptions
                             where sd.PropertyName == propertyName
                             select sd.Direction).DefaultIfEmpty(ListSortDirection.Descending).First();

            _resultsViewSource.SortDescriptions.Clear();
            _resultsViewSource.SortDescriptions.Add(new(propertyName,
                direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending));
        }

        private void ShowProperty()
        {
            var files = from result in _searcher.Results
                        where result.IsSelected.Value
                        select result.FilePath;

            foreach (var file in files)
            {
                FileOperate.ShowPropertyDialog(file);
            }
        }
    }
}
