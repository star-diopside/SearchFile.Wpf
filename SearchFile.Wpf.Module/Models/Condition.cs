using Prism.Mvvm;
using Reactive.Bindings;
using SearchFile.Wpf.Module.Properties;
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
        public ReactiveProperty<string?> TargetDirectory { get; } = new ReactiveProperty<string?>(
            Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))
        ).SetValidateNotifyError(dir => Directory.Exists(dir) ? null : Resources.DirectoryNotFoundMessage);

        public ReactiveProperty<string?> FileName { get; } = new();

        public ReactiveProperty<FileNameMatchType> MatchType { get; } = new(FileNameMatchType.Wildcard);

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
