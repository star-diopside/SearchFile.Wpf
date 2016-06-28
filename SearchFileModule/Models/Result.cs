using Prism.Mvvm;
using PropertyChanged;
using SearchFile.Module.Shell;
using System.IO;
using System.Windows.Media;

namespace SearchFile.Module.Models
{
    /// <summary>
    /// ファイル検索結果を表すクラス
    /// </summary>
    [ImplementPropertyChanged]
    public class Result : BindableBase
    {
        private ImageSource _iconSource;

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
        /// リスト項目が選択されているかどうかを示す値を取得または設定する。
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// ファイルに関連付けられたアイコンを取得する。
        /// </summary>
        public ImageSource IconSource => this._iconSource ?? (this._iconSource = ExtractIcon.ExtractFileIcon(this.FilePath, ExtractIcon.IconSize.Small));

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
