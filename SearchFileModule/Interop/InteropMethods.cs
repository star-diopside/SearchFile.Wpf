using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SearchFile.Module.Interop
{
    internal static class InteropMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern object SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SHGetFileInfo([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGetFileInfoFlags uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern object SHGetImageList(SHIL iImageList, ref Guid riid);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyIcon(IntPtr hIcon);
    }
}
