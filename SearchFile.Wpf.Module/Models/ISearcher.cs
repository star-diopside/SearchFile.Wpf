using Reactive.Bindings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SearchFile.Wpf.Module.Models
{
    /// <summary>
    /// ファイル検索処理を表すインターフェース
    /// </summary>
    public interface ISearcher
    {
        /// <summary>
        /// 検索結果の読み取り専用リストを取得する。
        /// </summary>
        ReadOnlyObservableCollection<Result> Results { get; }

        /// <summary>
        /// 検索中ディレクトリの読み取り専用プロパティを取得する。
        /// </summary>
        ReadOnlyReactiveProperty<string?> SearchingDirectory { get; }

        /// <summary>
        /// 検索中かどうかを示す読み取り専用プロパティを取得する。
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsSearching { get; }

        /// <summary>
        /// 検索結果が存在するかどうかを示す読み取り専用プロパティを取得する。
        /// </summary>
        ReadOnlyReactiveProperty<bool> ExistsResults { get; }

        /// <summary>
        /// 検索処理を行う。
        /// </summary>
        /// <param name="condition">検索条件を表すオブジェクト</param>
        /// <param name="cancellationToken">非同期処理のキャンセル通知を行うためのキャンセルトークン</param>
        /// <returns>非同期操作を表すタスクオブジェクト</returns>
        Task SearchAsync(ICondition condition, CancellationToken cancellationToken = default);

        /// <summary>
        /// 検索結果をクリアする。
        /// </summary>
        void Clear();

        /// <summary>
        /// 検索結果リストから指定されたデータを削除する。
        /// </summary>
        /// <param name="results">検索結果リストから削除するデータ</param>
        void Remove(IEnumerable<Result> results);

        /// <summary>
        /// 検索結果をファイルに保存する。
        /// </summary>
        /// <param name="fileName">保存するファイル名</param>
        void Save(string fileName);
    }
}
