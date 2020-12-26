using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Services.FileFilters
{
    public class TextFileFilter : IFilter
    {
        public string Name { get; } = "テキストファイル";
        public IEnumerable<string> Patterns { get; } = new[] { "*.txt" };
    }
}
