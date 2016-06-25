using System;

namespace SearchFile.Module.Interop.CommonControlObjects
{
    [Flags]
    internal enum ImageListDrawFlags : uint
    {
        ILD_NORMAL = 0x00000000,
        ILD_TRANSPARENT = 0x00000001,
        ILD_MASK = 0x00000010,
        ILD_IMAGE = 0x00000020,
        ILD_ROP = 0x00000040,
        ILD_BLEND25 = 0x00000002,
        ILD_BLEND50 = 0x00000004,
        ILD_OVERLAYMASK = 0x00000F00,
        ILD_PRESERVEALPHA = 0x00001000,
        ILD_SCALE = 0x00002000,
        ILD_DPISCALE = 0x00004000,
        ILD_ASYNC = 0x00008000,
        ILD_SELECTED = ILD_BLEND50,
        ILD_FOCUS = ILD_BLEND25,
        ILD_BLEND = ILD_BLEND50
    }
}
