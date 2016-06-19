using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SearchFile.Module.Interop.ShellObjects
{
    internal static class Methods
    {
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern object SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);

        internal static IShellItem CreateShellItem(string path)
        {
            Guid iid = typeof(IShellItem).GUID;
            return (IShellItem)SHCreateItemFromParsingName(path, null, ref iid);
        }
    }
}
