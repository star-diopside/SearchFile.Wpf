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
        /// <summary>
        /// 検索対象ディレクトリを取得または設定する。
        /// </summary>
        public ReactiveProperty<string?> TargetDirectory { get; } = new ReactiveProperty<string?>(
            Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))
        ).SetValidateNotifyError(dir => Directory.Exists(dir) ? null : Resources.DirectoryNotFoundMessage);

        /// <summary>
        /// 検索ファイル名を取得または設定する。
        /// </summary>
        public ReactiveProperty<string?> FileName { get; } = new();

        /// <summary>
        /// 検索パターンを取得または設定する。
        /// </summary>
        public ReactiveProperty<FileNameMatchType> MatchType { get; } = new(FileNameMatchType.Wildcard);

        /// <summary>
        /// ファイル検索処理を取得する。
        /// </summary>
        /// <returns>指定されたディレクトリのファイル一覧を返すデリゲート</returns>
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
