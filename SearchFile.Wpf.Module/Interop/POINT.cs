using System.Runtime.InteropServices;

namespace SearchFile.Wpf.Module.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }
}
