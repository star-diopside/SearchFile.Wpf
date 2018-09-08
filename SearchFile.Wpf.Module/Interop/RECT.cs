using System.Runtime.InteropServices;

namespace SearchFile.Wpf.Module.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}
