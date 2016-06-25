using System;
using System.Runtime.InteropServices;

namespace SearchFile.Module.Interop.CommonControlObjects
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }
}
