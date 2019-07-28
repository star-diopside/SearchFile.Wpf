using SearchFile.Wpf.Module.Models;
using System.Collections.Generic;
using System.Linq;

namespace SearchFile.Wpf.Module.Messaging
{
    public class DeleteFileMessage
    {
        public IEnumerable<Result> Results { get; set; } = Enumerable.Empty<Result>();
        public bool Recycle { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
