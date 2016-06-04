using CsvHelper;
using NLog;
using Prism.Mvvm;
using PropertyChanged;
using SearchFileModule.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchFile.Models
{
    [ImplementPropertyChanged]
    public class Searcher : BindableBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ObservableCollection<Result> Results { get; } = new ObservableCollection<Result>();

        public string Status { get; private set; }

        public bool IsSearching => this.CancellationTokenSource != null;

        public bool ExistsResults => this.Results.Any();

        private CancellationTokenSource CancellationTokenSource { get; set; }

        public Searcher()
        {
            this.Results.CollectionChanged += (s, e) => this.OnPropertyChanged(nameof(ExistsResults));
        }

        public async Task Search(Condition condition)
        {
            if (this.IsSearching)
            {
                throw new InvalidOperationException();
            }

            var directoryProgress = new Progress<string>(directory =>
            {
                this.Status = string.Format(Resources.SearchingDirectoryMessage, directory);
            });

            var resultProgress = new Progress<Result>(result => this.Results.Add(result));

            this.Results.Clear();

            try
            {
                using (this.CancellationTokenSource = new CancellationTokenSource())
                {
                    var token = this.CancellationTokenSource.Token;
                    await Task.Run(() => Search(condition.TargetDirectory, condition.GetSearchFileStrategy(),
                        directoryProgress, resultProgress, token), token);
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.Debug(ex, ex.Message);
            }
            finally
            {
                this.CancellationTokenSource = null;
            }

            this.Status = string.Format(Resources.SearchingResultMessage, this.Results.Count);
        }

        private static void Search(string path, Func<string, IEnumerable<string>> strategy,
            IProgress<string> directoryProgress, IProgress<Result> resultProgress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            directoryProgress.Report(path);

            try
            {
                var results = from file in strategy(path) select Result.Of(file);
                foreach (var result in results)
                {
                    resultProgress.Report(result);
                }

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    Search(directory, strategy, directoryProgress, resultProgress, token);
                }
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is PathTooLongException || ex is UnauthorizedAccessException)
            {
                logger.Debug(ex, ex.Message);
            }
        }

        public void Cancel()
        {
            this.CancellationTokenSource?.Cancel();
        }

        public void Clear()
        {
            this.Results.Clear();
            this.Status = Resources.ClearResultsMessage;
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
