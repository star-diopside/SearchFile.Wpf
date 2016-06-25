using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SearchFile.Module.Interop.ShellObjects
{
    [ComImport]
    [Guid(ComIid.IShellItem)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToHandler(IBindCtx pbc, ref Guid bhid, ref Guid riid);

        IShellItem GetParent();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetDisplayName(SIGDN sigdnName);

        uint GetAttributes(uint sfgaoMask);

        int Compare(IShellItem psi, uint hint);
    }
}
