using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Services.FileFilters
{
    public class Filter : IFilter
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<string> Patterns { get; set; } = new List<string>();
        IEnumerable<string> IFilter.Patterns => Patterns;
    }
}
