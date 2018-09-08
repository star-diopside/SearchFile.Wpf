using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Messaging.FileFilters
{
    public class Filter : IFilter
    {
        public string Name { get; set; }
        public ICollection<string> Patterns { get; set; } = new List<string>();
        IEnumerable<string> IFilter.Patterns => this.Patterns;
    }
}
