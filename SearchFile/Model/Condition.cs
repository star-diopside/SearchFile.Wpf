using PropertyChanged;
using SearchFile.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchFile.Model
{
    /// <summary>
    /// ファイル検索条件を表すクラス
    /// </summary>
    [ImplementPropertyChanged]
    public class Condition : INotifyDataErrorInfo
    {
        private Dictionary<string, IEnumerable<object>> errors = new Dictionary<string, IEnumerable<object>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => errors.Values.Any(e => e != null && e.Any());

        public IEnumerable GetErrors(string propertyName) => errors[propertyName];

        private string targetDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

        /// <summary>
        /// 検索対象ディレクトリを取得または設定する。
        /// </summary>
        public string TargetDirectory
        {
            get
            {
                return targetDirectory;
            }
            set
            {
                targetDirectory = value;

                if (Directory.Exists(targetDirectory))
                {
                    errors.Remove(nameof(TargetDirectory));
                }
                else
                {
                    errors[nameof(TargetDirectory)] = new[] { Resources.DirectoryNotFoundMessage };
                }

                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(TargetDirectory)));
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

        /// <summary>
        /// ファイル検索処理を取得する。
        /// </summary>
        public Func<string, IEnumerable<string>> GetSearchFileStrategy()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                return path => Directory.EnumerateFiles(path);
            }
            else
            {
                switch (this.MatchType)
                {
                    case FileNameMatchType.Wildcard:
                        return path => Directory.EnumerateFiles(path, this.FileName);
                    case FileNameMatchType.Regex:
                        var pattern = new Regex(this.FileName, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        return path => from file in Directory.EnumerateFiles(path)
                                       where pattern.IsMatch(file)
                                       select file;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
