using PropertyChanged;

namespace SearchFile.Model
{
    /// <summary>
    /// ファイル検索条件を表すクラス
    /// </summary>
    [ImplementPropertyChanged]
    public class Condition
    {
        /// <summary>
        /// 検索対象ディレクトリ
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 検索パターン
        /// </summary>
        public string MatchType { get; set; }
    }
}
