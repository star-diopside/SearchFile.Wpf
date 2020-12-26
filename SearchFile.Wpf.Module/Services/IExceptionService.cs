using System;

namespace SearchFile.Wpf.Module.Services
{
    public interface IExceptionService
    {
        void ShowDialog(Exception exception);
    }
}
