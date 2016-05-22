using System;
using System.Collections.Generic;

namespace SearchFile.Messaging
{
    public class SaveFileMessage
    {
        public string Path { get; set; }
        public ICollection<Filter> Filters { get; } = new List<Filter>();
        public Action<string> Callback { get; set; }

        public class Filter
        {
            public string Name { get; set; }
            public ICollection<string> Patterns { get; } = new List<string>();
        }
    }
}
