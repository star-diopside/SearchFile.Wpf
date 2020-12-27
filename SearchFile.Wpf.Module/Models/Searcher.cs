using CsvHelper;
using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchFile.Wpf.Module.Models
{
    public class Searcher : BindableBase, ISearcher
    {
        private readonly ILogger<Searcher> _logger;

        private readonly ReactiveProperty<string?> _latestSearchingDirectory = new();
        private readonly ReactiveProperty<CancellationTokenSource?> _cancellationTokenSource = new();
        private readonly ObservableCollection<Result> _results = new();

        public ReadOnlyObservableCollection<Result> Results { get; }

        public ReadOnlyReactiveProperty<string?> SearchingDirectory { get; }

        public ReadOnlyReactiveProperty<bool> IsSearching { get; }

        public ReadOnlyReactiveProperty<bool> ExistsResults { get; }

        public Searcher(ILogger<Searcher> logger)
        {
            _logger = logger;

            Results = new(_results);
            SearchingDirectory = _latestSearchingDirectory.ToReadOnlyReactiveProperty();
            IsSearching = _cancellationTokenSource.Select(s => s is not null).ToReadOnlyReactiveProperty();
            ExistsResults = Results.CollectionChangedAsObservable().Select(_ => Results.Any()).ToReadOnlyReactiveProperty();
        }

        public async Task SearchAsync(ICondition condition)
        {
            if (IsSearching.Value || condition.TargetDirectory.Value is not string targetDirectory)
            {
                throw new InvalidOperationException();
            }

            var directoryProgress = new Progress<string>(directory => _latestSearchingDirectory.Value = directory);
            var resultProgress = new Progress<Result>(_results.Add);

            _results.Clear();

            try
            {
                using (_cancellationTokenSource.Value = new())
                {
                    var token = _cancellationTokenSource.Value.Token;
                    await Task.Run(() => Search(targetDirectory, condition.GetSearchFileStrategy(),
                        directoryProgress, resultProgress, token), token);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, ex.Message);
            }
            finally
            {
                _cancellationTokenSource.Value = null;
            }

            _latestSearchingDirectory.Value = null;
        }

        private void Search(string path, Func<string, IEnumerable<string>> strategy,
            IProgress<string> directoryProgress, IProgress<Result> resultProgress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            directoryProgress.Report(path);

            try
            {
                var results = from file in strategy(path) select new Result(file);
                foreach (var result in results)
                {
                    resultProgress.Report(result);
                }

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    Search(directory, strategy, directoryProgress, resultProgress, token);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                _logger.LogDebug(ex, ex.Message);
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Value?.Cancel();
        }

        public void Clear()
        {
            _results.Clear();
        }

        public void Remove(IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                _results.Remove(result);
            }
        }

        public void Save(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".csv":
                    SaveCsv(fileName);
                    break;
                default:
                    SaveText(fileName);
                    break;
            }
        }

        private void SaveCsv(string fileName)
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(Results.Select(result => new
            {
                result.DirectoryName,
                result.FileName,
                result.Extension
            }));
        }

        private void SaveText(string fileName)
        {
            using var writer = new StreamWriter(fileName);

            foreach (var result in Results)
            {
                writer.WriteLine(result.FilePath);
            }
        }
    }
}
