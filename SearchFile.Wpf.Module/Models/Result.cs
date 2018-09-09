using Prism.Mvvm;
using Reactive.Bindings;
using SearchFile.Wpf.Module.Shell;
using System.IO;
using System.Windows.Media;

namespace SearchFile.Wpf.Module.Models
{
    /// <summary>
    /// ファイル検索結果を表すクラス
    /// </summary>
    public class Result : BindableBase
    {
        /// <summary>
        /// リスト項目が選択されているかどうかを示す値を取得または設定する。
        /// </summary>
        public ReactiveProperty<bool> IsSelected { get; } = new ReactiveProperty<bool>();

        /// <summary>
        /// ファイルパスを取得する。
        /// </summary>
        public string FilePath { get; }

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
        public ImageSource IconSource => ExtractIcon.ExtractFileIcon(this.FilePath, ExtractIcon.IconSize.Small);

        public Result(string path)
        {
            this.FilePath = path;
        }
    }
}
