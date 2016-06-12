using SearchFile.Messaging.FileFilters;
using System.Collections.Generic;

namespace SearchFile.Messaging
{
    public class SaveFileMessage
    {
        public string Path { get; set; }
        public ICollection<IFilter> Filters { get; } = new List<IFilter>();
    }
}
