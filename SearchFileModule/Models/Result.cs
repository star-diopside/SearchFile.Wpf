using Microsoft.WindowsAPICodePack.Shell;
using NLog;
using System.IO;
using System.Windows.Media;

namespace SearchFile.Models
{
    /// <summary>
    /// ファイル検索結果を表すクラス
    /// </summary>
    public class Result
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ファイル名を取得する。
        /// </summary>
        public string FileName => Path.GetFileName(this.FilePath);

        /// <summary>
        /// 拡張子を取得する。
        /// </summary>
        public string Extension => Path.GetExtension(this.FilePath);

        /// <summary>
        /// ディレクトリ名を取得する。
        /// </summary>
        public string DirectoryName => Path.GetDirectoryName(this.FilePath);

        /// <summary>
        /// ファイルに関連付けられたアイコンを取得する。
        /// </summary>
        public ImageSource IconSource
        {
            get
            {
                try
                {
                    return ShellFile.FromFilePath(this.FilePath).Thumbnail.SmallBitmapSource;
                }
                catch (ShellException ex)
                {
                    logger.Warn(ex, ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// ファイルパスを取得する。
        /// </summary>
        public string FilePath { get; }

        public Result(string path)
        {
            this.FilePath = path;
        }
    }
}
