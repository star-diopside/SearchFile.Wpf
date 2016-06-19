using SearchFile.Module.Models;
using System.Collections.Generic;

namespace SearchFile.Module.Messaging
{
    public class DeleteFileMessage
    {
        public IEnumerable<Result> Results { get; set; }
        public bool Recycle { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
