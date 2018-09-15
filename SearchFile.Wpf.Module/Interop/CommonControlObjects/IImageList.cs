using System;
using System.Runtime.InteropServices;

namespace SearchFile.Wpf.Module.Interop.CommonControlObjects
{
    [ComImport]
    [Guid(ComIid.IImageList)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IImageList
    {
        int Add(
            IntPtr hbmImage,
            IntPtr hbmMask);

        int ReplaceIcon(
            int i,
            IntPtr hicon);

        void SetOverlayImage(
            int iImage,
            int iOverlay);

        void Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

        int AddMasked(
            IntPtr hbmImage,
            uint crMask);

        void Draw(
            [In] ref IMAGELISTDRAWPARAMS pimldp);

        void Remove(int i);

        IntPtr GetIcon(
            int i,
            ImageListDrawFlags flags);

        IMAGEINFO GetImageInfo(int i);

        void Copy(
            int iDst,
            [MarshalAs(UnmanagedType.IUnknown)] object punkSrc,
            int iSrc,
            uint uFlags);

        [return: MarshalAs(UnmanagedType.Interface)]
        object Merge(
            int i1,
            [MarshalAs(UnmanagedType.IUnknown)] object punk2,
            int i2,
            int dx,
            int dy,
            ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object Clone(ref Guid riid);

        RECT GetImageRect(int i);

        void GetIconSize(
            out int cx,
            out int cy);

        void SetIconSize(
            int cx,
            int cy);

        int GetImageCount();

        void SetImageCount(uint uNewCount);

        uint SetBkColor(uint clrBk);

        uint GetBkColor();

        void BeginDrag(
            int iTrack,
            int dxHotspot,
            int dyHotspot);

        void EndDrag();

        void DragEnter(
            IntPtr hwndLock,
            int x,
            int y);

        void DragLeave(IntPtr hwndLock);

        void DragMove(
            int x,
            int y);

        void SetDragCursorImage(
            [MarshalAs(UnmanagedType.IUnknown)] object punk,
            int iDrag,
            int dxHotspot,
            int dyHotspot);

        void DragShowNolock(
            [MarshalAs(UnmanagedType.Bool)] bool fShow);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetDragImage(
            out POINT ppt,
            out POINT pptHotspot,
            ref Guid riid);

        uint GetItemFlags(int i);

        int GetOverlayImage(int iOverlay);
    }
}
