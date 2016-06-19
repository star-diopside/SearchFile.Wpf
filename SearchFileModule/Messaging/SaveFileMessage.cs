using SearchFile.Module.Messaging.FileFilters;
using System.Collections.Generic;

namespace SearchFile.Module.Messaging
{
    public class SaveFileMessage
    {
        public string Path { get; set; }
        public ICollection<IFilter> Filters { get; } = new List<IFilter>();
    }
}
