using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SearchFile.Model
{
    [ImplementPropertyChanged]
    public class Searcher
    {
        public ObservableCollection<Result> Results { get; } = new ObservableCollection<Result>();

        public async Task Search(Condition condition)
        {
            IProgress<Result> progress = new Progress<Result>(result => this.Results.Add(result));
            await Task.Run(() =>
            {
                var results = from file in Directory.EnumerateFileSystemEntries(condition.TargetDirectory)
                              select new Result(file);
                foreach (var result in results)
                {
                    progress.Report(result);
                }
            });
        }
    }
}
