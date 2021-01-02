using CsvHelper;
using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using Reactive.Bindings;
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

        private readonly ObservableCollection<Result> _results = new();
        private readonly ReactivePropertySlim<string?> _searchingDirectory = new();

        public ReadOnlyObservableCollection<Result> Results { get; }

        public ReadOnlyReactivePropertySlim<string?> SearchingDirectory { get; }

        public Searcher(ILogger<Searcher> logger)
        {
            _logger = logger;

            Results = new(_results);
            SearchingDirectory = _searchingDirectory.ToReadOnlyReactivePropertySlim();
        }

        public async Task SearchAsync(ICondition condition, CancellationToken cancellationToken = default)
        {
            if (condition.TargetDirectory.Value is not string targetDirectory)
            {
                throw new InvalidOperationException();
            }

            var directoryProgress = new Progress<string>(directory => _searchingDirectory.Value = directory);
            var resultProgress = new Progress<IEnumerable<Result>>(results => _results.AddRange(results));

            _results.Clear();

            try
            {
                await Task.Run(() => Search(targetDirectory, condition.GetSearchFileStrategy(),
                    directoryProgress, resultProgress, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, ex.Message);
            }

            _searchingDirectory.Value = null;
        }

        private void Search(string path, Func<string, IEnumerable<string>> strategy,
            IProgress<string> directoryProgress, IProgress<IEnumerable<Result>> resultProgress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            directoryProgress.Report(path);

            try
            {
                var results = strategy(path).Select(file => new Result(file));
                if (results.Any())
                {
                    resultProgress.Report(results);
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
