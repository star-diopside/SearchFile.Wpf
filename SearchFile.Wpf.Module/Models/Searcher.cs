using CsvHelper;
using NLog;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchFile.Wpf.Module.Models
{
    public class Searcher : BindableBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ObservableCollection<Result> Results { get; } = new ObservableCollection<Result>();

        public ReadOnlyReactiveProperty<string> SearchingDirectory { get; }

        public ReadOnlyReactiveProperty<bool> IsSearching { get; }

        public ReadOnlyReactiveProperty<bool> ExistsResults { get; }

        private readonly ReactiveProperty<string> latestSearchingDirectory = new ReactiveProperty<string>();

        private readonly ReactiveProperty<CancellationTokenSource> cancellationTokenSource = new ReactiveProperty<CancellationTokenSource>();

        public Searcher()
        {
            this.SearchingDirectory = this.latestSearchingDirectory.ToReadOnlyReactiveProperty();
            this.IsSearching = this.cancellationTokenSource.Select(s => s != null).ToReadOnlyReactiveProperty();
            this.ExistsResults = this.Results.CollectionChangedAsObservable().Select(_ => this.Results.Any()).ToReadOnlyReactiveProperty();
        }

        public async Task SearchAsync(Condition condition)
        {
            if (this.IsSearching.Value)
            {
                throw new InvalidOperationException();
            }

            var directoryProgress = new Progress<string>(directory => this.latestSearchingDirectory.Value = directory);
            var resultProgress = new Progress<Result>(this.Results.Add);

            this.Results.Clear();

            try
            {
                using (this.cancellationTokenSource.Value = new CancellationTokenSource())
                {
                    var token = this.cancellationTokenSource.Value.Token;
                    await Task.Run(() => Search(condition.TargetDirectory.Value, condition.GetSearchFileStrategy(),
                        directoryProgress, resultProgress, token), token);
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.Debug(ex, ex.Message);
            }
            finally
            {
                this.cancellationTokenSource.Value = null!;
            }

            this.latestSearchingDirectory.Value = null!;
        }

        private static void Search(string path, Func<string, IEnumerable<string>> strategy,
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
                logger.Debug(ex, ex.Message);
            }
        }

        public void Cancel()
        {
            this.cancellationTokenSource.Value?.Cancel();
        }

        public void Clear()
        {
            this.Results.Clear();
        }

        public void Save(string fileName)
        {
            switch (Path.GetExtension(fileName)?.ToLower())
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
            using (var writer = new CsvWriter(new StreamWriter(fileName, false, Encoding.UTF8)))
            {
                writer.WriteField("DirectoryName");
                writer.WriteField("FileName");
                writer.WriteField("Extension");
                writer.NextRecord();

                foreach (var result in this.Results)
                {
                    writer.WriteField(result.DirectoryName);
                    writer.WriteField(result.FileName);
                    writer.WriteField(result.Extension);
                    writer.NextRecord();
                }
            }
        }

        private void SaveText(string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                foreach (var result in this.Results)
                {
                    writer.WriteLine(result.FilePath);
                }
            }
        }
    }
}
