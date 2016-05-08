using PropertyChanged;
using System.IO;

namespace SearchFile.Model
{
    /// <summary>
    /// ファイル検索結果を表すクラス
    /// </summary>
    [ImplementPropertyChanged]
    public class Result
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 拡張子
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// ディレクトリ名
        /// </summary>
        public string DirectoryName { get; set; }

        public Result()
        {
        }

        public Result(string path)
        {
            this.DirectoryName = Path.GetDirectoryName(path);
            this.FileName = Path.GetFileName(path);
            this.Extension = Path.GetExtension(path);
        }
    }
}
