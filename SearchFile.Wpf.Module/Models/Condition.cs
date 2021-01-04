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

        public ReactivePropertySlim<bool> IsMatchFile { get; } = new(true);

        public ReactivePropertySlim<bool> IsMatchDirectory { get; } = new(false);

        public Func<string, IEnumerable<string>> GetSearchFileStrategy()
        {
            var fileName = FileName.Value;
            var matchType = MatchType.Value;
            var isMatchFile = IsMatchFile.Value;
            var isMatchDirectory = IsMatchDirectory.Value;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return path => EnumerateFileSystemEntries(path, "*", isMatchFile, isMatchDirectory);
            }
            else if (matchType == FileNameMatchType.Wildcard)
            {
                return path => EnumerateFileSystemEntries(path, fileName, isMatchFile, isMatchDirectory);
            }
            else if (matchType == FileNameMatchType.Regex)
            {
                var pattern = new Regex(fileName, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                return path => from file in EnumerateFileSystemEntries(path, "*", isMatchFile, isMatchDirectory)
                               where pattern.IsMatch(file)
                               select file;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern,
                                                               bool isMatchFile, bool isMatchDirectory)
        {
            return isMatchFile
                ? isMatchDirectory
                    ? Directory.EnumerateFileSystemEntries(path, searchPattern)
                    : Directory.EnumerateFiles(path, searchPattern)
                : isMatchDirectory
                    ? Directory.EnumerateDirectories(path, searchPattern)
                    : Enumerable.Empty<string>();
        }
    }
}
