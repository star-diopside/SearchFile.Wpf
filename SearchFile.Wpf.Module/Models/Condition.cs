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
    public class Condition : BindableBase
    {
        /// <summary>
        /// 検索対象ディレクトリを取得または設定する。
        /// </summary>
        public ReactiveProperty<string> TargetDirectory { get; } = new ReactiveProperty<string>(
            Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))!)
            .SetValidateNotifyError(dir => Directory.Exists(dir) ? null : Resources.DirectoryNotFoundMessage);

        /// <summary>
        /// 検索ファイル名を取得または設定する。
        /// </summary>
        public ReactiveProperty<string> FileName { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// 検索パターンを取得または設定する。
        /// </summary>
        public ReactiveProperty<FileNameMatchType> MatchType { get; } = new ReactiveProperty<FileNameMatchType>(FileNameMatchType.Wildcard);

        /// <summary>
        /// ファイル名検索パターン列挙子
        /// </summary>
        public enum FileNameMatchType
        {
            Wildcard,
            Regex
        }

        /// <summary>
        /// ファイル検索処理を取得する。
        /// </summary>
        /// <returns>指定されたディレクトリのファイル一覧を返すデリゲート</returns>
        public Func<string, IEnumerable<string>> GetSearchFileStrategy()
        {
            var fileName = this.FileName.Value;

            switch (this.MatchType.Value)
            {
                case FileNameMatchType.Wildcard:
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        return Directory.EnumerateFiles;
                    }
                    else
                    {
                        return path => Directory.EnumerateFiles(path, fileName);
                    }
                case FileNameMatchType.Regex:
                    var pattern = new Regex(fileName, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    return path => from file in Directory.EnumerateFiles(path)
                                   where pattern.IsMatch(file)
                                   select file;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
