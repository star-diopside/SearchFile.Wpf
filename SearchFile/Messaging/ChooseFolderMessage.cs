using System;

namespace SearchFile.Messaging
{
    public class ChooseFolderMessage
    {
        public string Path { get; set; }
        public Action<string> Callback { get; set; }
    }
}
