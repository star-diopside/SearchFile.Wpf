using System.IO;

namespace SearchFile.Models
{
    /// <summary>
    /// ファイル検索結果を表すクラス
    /// </summary>
    public class Result
    {
        /// <summary>
        /// ファイル名を取得または設定する。
        /// </summary>
        public string FileName => Path.GetFileName(this.FilePath);

        /// <summary>
        /// 拡張子を取得または設定する。
        /// </summary>
        public string Extension => Path.GetExtension(this.FilePath);

        /// <summary>
        /// ディレクトリ名を取得または設定する。
        /// </summary>
        public string DirectoryName => Path.GetDirectoryName(this.FilePath);

        /// <summary>
        /// ファイルパスを取得または設定する。
        /// </summary>
        public string FilePath { get; }

        public Result(string path)
        {
            this.FilePath = path;
        }
    }
}
