using System;

namespace SearchFile.Messaging
{
    public class ExceptionMessage
    {
        public Exception Exception { get; }

        public ExceptionMessage(Exception ex)
        {
            this.Exception = ex;
        }
    }
}
