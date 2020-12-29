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
        private readonly ReactiveProperty<bool> _isSearching = new();
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
            IsSearching = _isSearching.ToReadOnlyReactiveProperty();
            ExistsResults = Results.CollectionChangedAsObservable().Select(_ => Results.Any()).ToReadOnlyReactiveProperty();
        }

        public async Task SearchAsync(ICondition condition, CancellationToken cancellationToken = default)
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
                _isSearching.Value = true;
                await Task.Run(() => Search(targetDirectory, condition.GetSearchFileStrategy(),
                    directoryProgress, resultProgress, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, ex.Message);
            }
            finally
            {
                _isSearching.Value = false;
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
                foreach (var result in strategy(path).Select(file => new Result(file)))
                {
                    resultProgress.Report(result);
                }

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    Search(directory, strategy, directoryProgress, resultProgress, token);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _logger.LogDebug(ex, ex.Message);
            }
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
