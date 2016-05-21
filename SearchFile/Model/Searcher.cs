using NLog;
using PropertyChanged;
using SearchFile.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchFile.Model
{
    [ImplementPropertyChanged]
    public class Searcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ObservableCollection<Result> Results { get; } = new ObservableCollection<Result>();

        public string Status { get; private set; }

        public bool IsSearching => this.CancellationTokenSource != null;

        private CancellationTokenSource CancellationTokenSource { get; set; }

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

        public void SetError(Exception ex)
        {
            this.Status = Resources.SearchingErrorMessage;
        }
    }
}
