using Prism.Mvvm;
using PropertyChanged;
using SearchFile.Wpf.Module.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchFile.Wpf.Module.Models
{
    /// <summary>
    /// ファイル検索条件を表すクラス
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Condition : BindableBase, INotifyDataErrorInfo
    {
        private ErrorsContainer<string> errorsContainer;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => this.errorsContainer.HasErrors;

        public IEnumerable GetErrors(string propertyName) => this.errorsContainer.GetErrors(propertyName);

        private string _targetDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

        /// <summary>
        /// 検索対象ディレクトリを取得または設定する。
        /// </summary>
        public string TargetDirectory
        {
            get
            {
                return this._targetDirectory;
            }
            set
            {
                this._targetDirectory = value;

                if (Directory.Exists(this._targetDirectory))
                {
                    this.errorsContainer.ClearErrors(nameof(TargetDirectory));
                }
                else
                {
                    this.errorsContainer.SetErrors(nameof(TargetDirectory), new[] { Resources.DirectoryNotFoundMessage });
                }
            }
        }

        /// <summary>
        /// 検索ファイル名を取得または設定する。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 検索パターンを取得または設定する。
        /// </summary>
        public FileNameMatchType MatchType { get; set; } = FileNameMatchType.Wildcard;

        /// <summary>
        /// ファイル名検索パターン列挙子
        /// </summary>
        public enum FileNameMatchType
        {
            Wildcard,
            Regex
        }

        public Condition()
        {
            this.errorsContainer = new ErrorsContainer<string>(
                propertyName => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName)));
        }

        /// <summary>
        /// ファイル検索処理を取得する。
        /// </summary>
        /// <returns>指定されたディレクトリのファイル一覧を返すデリゲート</returns>
        public Func<string, IEnumerable<string>> GetSearchFileStrategy()
        {
            var fileName = this.FileName;

            switch (this.MatchType)
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
