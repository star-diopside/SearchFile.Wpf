using SearchFile.Models;
using System.Collections.Generic;

namespace SearchFile.Messaging
{
    public class DeleteFileMessage
    {
        public IEnumerable<Result> Results { get; set; }
        public bool Recycle { get; set; }
    }
}
