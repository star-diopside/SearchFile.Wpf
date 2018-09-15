using System;
using System.Runtime.InteropServices;

namespace SearchFile.Wpf.Module.Interop.CommonControlObjects
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGELISTDRAWPARAMS
    {
        public uint cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;
        public int yBitmap;
        public uint rgbBk;
        public uint rgbFg;
        public ImageListDrawFlags fStyle;
        public uint dwRop;
        public uint fState;
        public uint Frame;
        public uint crEffect;
    }
}
