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
        public ReactivePropertySlim<bool> IsSelected { get; } = new();

        /// <summary>
        /// ファイルパスを取得する。
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// ファイル名を取得する。
        /// </summary>
        public string FileName => Path.GetFileName(FilePath);

        /// <summary>
        /// 拡張子を取得する。
        /// </summary>
        public string Extension => Path.GetExtension(FilePath);

        /// <summary>
        /// ディレクトリ名を取得する。
        /// </summary>
        public string? DirectoryName => Path.GetDirectoryName(FilePath);

        /// <summary>
        /// ファイルに関連付けられたアイコンを取得する。
        /// </summary>
        public ImageSource IconSource => ExtractIcon.ExtractFileIcon(FilePath, ExtractIcon.IconSize.Small);

        public Result(string path)
        {
            FilePath = path;
        }
    }
}
