using SearchFile.Wpf.Module.Messaging.FileFilters;
using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Messaging
{
    public class SaveFileMessage
    {
        public string Path { get; set; }
        public ICollection<IFilter> Filters { get; } = new List<IFilter>();
    }
}
