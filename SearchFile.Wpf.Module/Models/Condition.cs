using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchFile.Wpf.Module.Models
{
    /// <summary>
    /// ファイル検索条件を表すクラス
    /// </summary>
    public class Condition : BindableBase, ICondition
    {
        public ReactivePropertySlim<string?> TargetDirectory { get; } = new(
            initialValue: Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
            mode: ReactivePropertyMode.Default & ~ReactivePropertyMode.DistinctUntilChanged);

        public ReactivePropertySlim<string?> FileName { get; } = new();

        public ReactivePropertySlim<FileNameMatchType> MatchType { get; } = new(FileNameMatchType.Wildcard);

        public Func<string, IEnumerable<string>> GetSearchFileStrategy()
        {
            var fileName = FileName.Value;
            var matchType = MatchType.Value;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Directory.EnumerateFiles;
            }
            else if (matchType == FileNameMatchType.Wildcard)
            {
                return path => Directory.EnumerateFiles(path, fileName);
            }
            else if (matchType == FileNameMatchType.Regex)
            {
                var pattern = new Regex(fileName, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                return path => from file in Directory.EnumerateFiles(path)
                               where pattern.IsMatch(file)
                               select file;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
