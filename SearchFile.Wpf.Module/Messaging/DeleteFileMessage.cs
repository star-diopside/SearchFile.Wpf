using SearchFile.Wpf.Module.Models;
using System.Collections.Generic;

namespace SearchFile.Wpf.Module.Messaging
{
    public class DeleteFileMessage
    {
        public IEnumerable<Result> Results { get; set; }
        public bool Recycle { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
