using Reactive.Bindings;
using System;
using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Models
{
    /// <summary>
    /// ファイル検索条件を表すインターフェース
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// 検索対象ディレクトリを表すプロパティを取得する。
        /// </summary>
        ReactivePropertySlim<string?> TargetDirectory { get; }

        /// <summary>
        /// 検索ファイル名を表すプロパティを取得する。
        /// </summary>
        ReactivePropertySlim<string?> FileName { get; }

        /// <summary>
        /// 検索パターンを表すプロパティを取得する。
        /// </summary>
        ReactivePropertySlim<FileNameMatchType> MatchType { get; }

        /// <summary>
        /// 検索結果にファイル名を含むかどうかを示すプロパティを取得する。
        /// </summary>
        ReactivePropertySlim<bool> IsMatchFile { get; }

        /// <summary>
        /// 検索結果にディレクトリ名を含むかどうかを示すプロパティを取得する。
        /// </summary>
        ReactivePropertySlim<bool> IsMatchDirectory { get; }

        /// <summary>
        /// ファイル検索処理を取得する。
        /// </summary>
        /// <returns>指定されたディレクトリのファイル一覧を返すデリゲート</returns>
        Func<string, IEnumerable<string>> GetSearchFileStrategy();
    }
}
